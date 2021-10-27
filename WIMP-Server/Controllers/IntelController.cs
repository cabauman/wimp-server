using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WIMP_Server.Data;
using WIMP_Server.DataServices.Http;
using WIMP_Server.Dtos;
using WIMP_Server.Dtos.Esi;
using WIMP_Server.Models;
using WIMP_Server.Searching;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IntelController : ControllerBase
    {
        private readonly ILogger<IntelController> _logger;
        private readonly IWimpRepository _repository;
        private readonly IEsiDataClient _esi;
        private readonly IMapper _mapper;

        public IntelController(ILogger<IntelController> logger, IWimpRepository repository, IEsiDataClient esi, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _esi = esi;
            _mapper = mapper;

            _logger.LogTrace($"Created {nameof(IntelController)}");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> ReportIntel(CreateIntelDto intel)
        {
            _logger.LogTrace($"ReportIntel {JsonSerializer.Serialize(intel)}");

            try
            {
                var intelCandidates = await FindBestShipCharacterSystemMatches(intel.Message)
                    .ConfigureAwait(true);

                if (intelCandidates != null)
                {
                    CreateIntel(intelCandidates, intel);
                }
                else
                {
                    _logger.LogWarning($"Couldn't determine best entity matches from: {intel.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to create intel report: {intel.Message}");
                return BadRequest($"Unable to create intel report: {intel.Message}");
            }

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadIntelDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ReadIntelDto>> GetIntel()
        {
            var result = _repository.GetIntel()
                .Select(i => AssociateIntelReport(i));

            return Ok(result);
        }

        [HttpGet]
        [Route("{intelId}")]
        [ProducesResponseType(typeof(ReadIntelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ReadIntelDto> GetIntelById(int intelId)
        {
            var intel = _repository.GetIntelById(intelId);
            if (intel == null)
            {
                return NotFound($"No intel with Id: {intelId}");
            }

            return Ok(AssociateIntelReport(intel));
        }

        private ReadIntelDto AssociateIntelReport(Intel intel)
        {
            intel.StarSystem = _repository.GetStarSystemWithId(intel.StarSystemId);

            if (intel.CharacterId.HasValue)
            {
                intel.Character = _repository.GetCharacterWithId(intel.CharacterId.Value);
            }
            if (intel.ShipId.HasValue)
            {
                intel.Ship = _repository.GetShipWithId(intel.ShipId.Value);
            }

            return _mapper.Map<ReadIntelDto>(intel);
        }

        private async Task<IntelCandidates> FindBestShipCharacterSystemMatches(string intel)
        {
            var originalIntel = new string(intel);

            // NOTE: Generate initial search candidates
            var searchCandidates = SearchMessageUtilities.GenerateSearchCandidates(intel);

            // NOTE: Search ESI for matching candidates
            var esiSearchResults = await _esi.UniverseSearchNames(searchCandidates)
                .ConfigureAwait(true);

            var searchShips = _repository.FindShipsWithNames(searchCandidates);
            var searchCharacters = _mapper.Map<IEnumerable<Character>>(esiSearchResults.Characters);

            // NOTE: Remove any characters with same name as a ship where ship name
            // is only a single word. This will remove some false positive matches.
            searchCharacters = searchCharacters.Where(character =>
                !searchShips.Any(ship => !ship.Name.Contains(' ') && ship.Name == character.Name)
            );

            // NOTE: Find best character matches
            var bestCharacterMatches = FindBestCharacterMatches(searchCharacters, ref intel);

            // NOTE: Find best ship matches
            var bestShipMatches = FindBestShipMatches(searchShips, ref intel);

            // NOTE: Find best system matches
            // TODO: Search for systems in our own database instead?
            var searchSystems = _mapper.Map<IEnumerable<StarSystem>>(esiSearchResults.Systems);
            var bestSystemMatches = FindBestStarSystemMatches(searchSystems, ref intel);

            // NOTE: Check for clear/clr
            var searchClearTerms = new string[] { "clr", "clear" };
            var clearMatches = SearchMessageUtilities.ExtractNamesFromIntelStringDescendingLength(searchClearTerms, ref intel);
            var containsClear = clearMatches.Any();

            // NOTE: Check for nv
            var searchNoVisualTerms = new string[] { "nv" };
            var noVisualMatches = SearchMessageUtilities.ExtractNamesFromIntelStringDescendingLength(searchNoVisualTerms, ref intel);
            var containsNoVisual = noVisualMatches.Any();

            // NOTE: Check for spike
            var searchSpikeTerms = new string[] { "spike " };
            var spikeMatches = SearchMessageUtilities.ExtractNamesFromIntelStringDescendingLength(searchSpikeTerms, ref intel);
            var containsSpike = spikeMatches.Any();

            if (!string.IsNullOrWhiteSpace(intel))
            {
                _logger.LogWarning($"Unable to parse remaining intel: '{intel.Trim()}' of '{originalIntel}'");
            }

            var intelCandidates = new IntelCandidates
            {
                Ships = bestShipMatches,
                StarSystems = bestSystemMatches,
                Characters = bestCharacterMatches,
                ContainsClear = containsClear,
                ContainsNoVisual = containsNoVisual,
                ContainsSpike = containsSpike,
            };

            _logger.LogInformation($"Intel candidates: {JsonSerializer.Serialize(intelCandidates)}");

            return intelCandidates;
        }

        private static IEnumerable<Ship> FindBestShipMatches(IEnumerable<Ship> searchShips, ref string intel)
        {
            var shipNames = searchShips.Select(s => s.Name);

            var bestShipMatches = SearchMessageUtilities
                .ExtractNamesFromIntelStringDescendingLength(shipNames, ref intel);

            return bestShipMatches.Select(shipName =>
                searchShips.FirstOrDefault(ship =>
                    string.Equals(shipName, ship.Name, StringComparison.InvariantCultureIgnoreCase)));
        }

        private static IEnumerable<Character> FindBestCharacterMatches(IEnumerable<Character> searchCharacters, ref string intel)
        {
            var characterNames = searchCharacters.Select(character => character.Name);
            var bestCharacterMatches = SearchMessageUtilities
                .ExtractNamesFromIntelStringDescendingLength(characterNames, ref intel, true);

            return bestCharacterMatches.Select(characterName =>
                searchCharacters.FirstOrDefault(character =>
                    string.Equals(characterName, character.Name, StringComparison.InvariantCulture)));
        }

        private static IEnumerable<StarSystem> FindBestStarSystemMatches(IEnumerable<StarSystem> searchStarSystems, ref string intel)
        {
            var starSystemNames = searchStarSystems.Select(starSystem => starSystem.Name);
            var bestStarSystemMatches = SearchMessageUtilities
                .ExtractNamesFromIntelStringDescendingLength(starSystemNames, ref intel, true);

            return bestStarSystemMatches.Select(starSystemName =>
                searchStarSystems.FirstOrDefault(starSystem =>
                    string.Equals(starSystemName, starSystem.Name, StringComparison.InvariantCulture)));
        }

        private void CreateIntelForCharacterAndShipPairs(DateTime timestamp, StarSystem starSystem, IEnumerable<Tuple<Character, Ship>> charactersAndShips)
        {
            foreach (var characterAndShip in charactersAndShips)
            {
                var (character, ship) = characterAndShip;

                if (_repository.GetIntelForCharacterWithIdAtTimestamp(character.CharacterId, timestamp) != null)
                {
                    _logger.LogInformation($"Duplicate intel for {character.Name} with ship {ship?.Name ?? "(null)"} in {starSystem?.Name ?? "(null)"}");
                    continue;
                }

                if (!_repository.HasCharacter(character.CharacterId))
                {
                    _repository.CreateCharacter(character);
                }

                if (ship != null && !_repository.HasShip(ship.ShipId))
                {
                    _repository.CreateShip(ship);
                }

                if (starSystem == null && ship != null)
                {
                    var existingIntel = _repository.GetMostRecentIntelForCharacterWithId(character.CharacterId);
                    if (existingIntel == null) continue;

                    _logger.LogInformation($"Updating intel {existingIntel.Id} with ship {ship.Name}");

                    existingIntel.ShipId = ship.ShipId;
                }
                else if (starSystem != null)
                {
                    if (ship == null)
                    {
                        var recentIntelForCharacter = _repository.GetMostRecentIntelForCharacterWithId(character.CharacterId);
                        if (recentIntelForCharacter != null && timestamp - recentIntelForCharacter.Timestamp <= TimeSpan.FromMinutes(10))
                        {
                            ship = recentIntelForCharacter.Ship;
                        }
                    }

                    var createdIntel = new Intel
                    {
                        Timestamp = timestamp,
                        CharacterId = character.CharacterId,
                        StarSystemId = starSystem.StarSystemId,
                        ShipId = ship?.ShipId,
                    };

                    _logger.LogInformation($"Creating intel for {character.Name} with ship {ship?.Name ?? "(null)"} in {starSystem.Name}");

                    _repository.CreateIntel(createdIntel);
                }
                else
                {
                    _logger.LogWarning($"Unable to create intel for {character.Name} with ship {ship?.Name ?? "(null)"} in {starSystem?.Name ?? "(null)"}");
                }
            }
        }

        private void CreateIntel(IntelCandidates reportCandidates, CreateIntelDto intel)
        {
            var characters = reportCandidates.Characters;
            var ships = reportCandidates.Ships;
            var starSystem = reportCandidates.StarSystems.FirstOrDefault();

            if (starSystem != null && !_repository.HasStarSystem(starSystem.StarSystemId))
            {
                _repository.CreateStarSystem(starSystem);
            }

            if (!DateTime.TryParse(intel.Timestamp, out var timestamp))
            {
                _logger.LogWarning($"Couldn't parse intel timestamp: {intel.Timestamp}");
            }

            if (characters.Any())
            {
                var characterShipPairs = characters
                    .Select((c, i) => new Tuple<Character, Ship>(c, ships.ElementAtOrDefault(i)));
                CreateIntelForCharacterAndShipPairs(timestamp, starSystem, characterShipPairs);
            }
            else if (starSystem != null && (reportCandidates.ContainsClear || reportCandidates.ContainsSpike))
            {
                var createdIntel = new Intel
                {
                    Timestamp = timestamp,
                    StarSystemId = starSystem.StarSystemId,
                    IsClear = reportCandidates.ContainsClear,
                    IsSpike = reportCandidates.ContainsSpike,
                };

                _logger.LogInformation($"Creating intel for {starSystem.Name} with status: clear({reportCandidates.ContainsClear}) spike({reportCandidates.ContainsSpike})");

                _repository.CreateIntel(createdIntel);
            }

            _repository.SaveChanges();
        }

        private class IntelCandidates
        {
            public IEnumerable<Ship> Ships { get; set; }
            public IEnumerable<Character> Characters { get; set; }
            public IEnumerable<StarSystem> StarSystems { get; set; }
            public bool ContainsNoVisual { get; set; }
            public bool ContainsClear { get; set; }
            public bool ContainsSpike { get; set; }
        }
    }
}