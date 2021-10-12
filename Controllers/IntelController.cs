using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WIMP_Server.Data;
using WIMP_Server.DataServices.Http;
using WIMP_Server.Dtos;
using WIMP_Server.Dtos.Esi;
using WIMP_Server.Models;

namespace WIMP_Server.Controllers
{
    /// <summary>
    /// For reporting intel.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class IntelController : ControllerBase
    {
        private readonly ILogger<IntelController> _logger;
        private readonly IWimpRepository _repository;
        private readonly IEsiDataClient _esi;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize a new IntelController instance.
        /// </summary>
        /// <param name="logger"></param>
        public IntelController(ILogger<IntelController> logger, IWimpRepository repository, IEsiDataClient esi, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _esi = esi;
            _mapper = mapper;

            _logger.LogTrace($"Created {nameof(IntelController)}");
        }

        /// <summary>
        /// Report intel posted in intel channel.
        /// </summary>
        /// <param name="intel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost]
        public async Task<ActionResult> ReportIntel(CreateIntelDto intel)
        {
            _logger.LogTrace($"ReportIntel {JsonSerializer.Serialize(intel)}");

            try
            {
                var reportCandidates = await FindBestShipCharacterSystemMatches(intel.Message);
                if (reportCandidates != null)
                {
                    CreateIntel(reportCandidates, intel);
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

            // TODO: Strip away any noise (people chatting in intel)
            // TODO: Handle SPIKE and +N reports
            // TODO: Handle nv
            // TODO: Handle ship abbreviations e.g. vni, oni, cane
            // TODO: Handle some known system abbreviations e.g. GJ0, JW-, A-8
            // TODO: Handle clr

            return Ok();
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReadIntelDto>> GetIntel()
        {
            var result = _repository.GetIntel()
                .Select(i => AssociateIntelReport(i));

            return Ok(result);
        }

        [HttpGet]
        [Route("{intelId}")]
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
            intel.Character = _repository.GetCharacterWithId(intel.CharacterId);
            intel.Ship = _repository.GetShipWithId(intel.ShipId ?? -1);

            return _mapper.Map<ReadIntelDto>(intel);
        }

        private IEnumerable<string> GenerateCandidates(string intel)
        {
            intel = intel.Replace("*", String.Empty);
            intel = intel.Replace("/", String.Empty);
            intel = intel.Replace("?", String.Empty);
            intel = intel.Replace(",", String.Empty);

            var parts = intel
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.Length > 2)
                .Distinct()
                .ToArray();

            var candidates = new List<string>();
            for (var p1 = 0; p1 < parts.Length; ++p1)
            {
                candidates.Add(parts[p1]);

                var sb = new StringBuilder(parts[p1]);
                for (var p2 = p1 + 1; p2 < parts.Length; ++p2)
                {
                    sb.Append($" {parts[p2]}");
                    candidates.Add(sb.ToString());
                }
            }

            return candidates;
        }

        private IEnumerable<ReadShipDto> GetShipsFromIntelString(IEnumerable<Ship> matchingShips, IEnumerable<ReadShipDto> ships, ref string intel)
        {
            var candidates = GenerateCandidates(intel);

            var bestCandidate = candidates
                .Where(c => matchingShips?.Any(s => String.Equals(s.Name, c, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                .OrderByDescending(s => s.Length)
                .FirstOrDefault();

            if (bestCandidate != null)
            {
                var bestShipMatch = matchingShips
                    .FirstOrDefault(mc => String.Equals(mc.Name, bestCandidate, StringComparison.InvariantCultureIgnoreCase));

                var ship = _mapper.Map<ReadShipDto>(bestShipMatch);

                intel = intel.Replace(bestCandidate, string.Empty);
                return GetShipsFromIntelString(matchingShips, ships.Append(ship), ref intel);
            }
            else
            {
                return ships;
            }
        }

        private IEnumerable<ReadCharacterDto> GetCharactersFromIntelString(IEnumerable<EsiNameIdPairDto> matchingCharacters, IEnumerable<ReadCharacterDto> characters, ref string intel)
        {
            var candidates = GenerateCandidates(intel);

            var bestCharacterMatch = candidates
                .Where(c => matchingCharacters?.Any(s => s.Name == c) ?? false)
                .OrderByDescending(s => s.Length)
                .FirstOrDefault();


            if (bestCharacterMatch != null)
            {
                var character = _mapper.Map<ReadCharacterDto>(matchingCharacters.FirstOrDefault(c => c.Name == bestCharacterMatch));

                intel = intel.Replace(bestCharacterMatch, string.Empty);
                return GetCharactersFromIntelString(matchingCharacters, characters.Append(character), ref intel);
            }
            else
            {
                return characters;
            }
        }

        private ReadStarSystemDto GetSystemFromIntelString(IEnumerable<EsiNameIdPairDto> matchingSystems, IEnumerable<ReadStarSystemDto> systems, ref string intel)
        {
            var candidates = GenerateCandidates(intel);

            var bestSystemMatch = candidates
                .Where(c => matchingSystems?.Any(s => s.Name == c) ?? false)
                .OrderByDescending(s => s.Length)
                .FirstOrDefault();

            if (bestSystemMatch != null)
            {
                var system = _mapper.Map<ReadStarSystemDto>(matchingSystems.FirstOrDefault(s => s.Name == bestSystemMatch));

                intel = intel.Replace(bestSystemMatch, string.Empty);
                return GetSystemFromIntelString(matchingSystems, systems.Append(system), ref intel);
            }
            else
            {
                return systems.FirstOrDefault();
            }
        }

        private async Task<ReportCandidatesDto> FindBestShipCharacterSystemMatches(string intel)
        {
            var originalIntel = new String(intel);
            var candidates = GenerateCandidates(intel);

            var shipTypes = _repository.FindShipsWithNames(candidates);
            var bestShipMatches = GetShipsFromIntelString(shipTypes, new List<ReadShipDto>(), ref intel);

            candidates = GenerateCandidates(intel);
            var result = await _esi.UniverseSearchNames(candidates);
            if (result == null) return null;

            var bestCharacterMatches = GetCharactersFromIntelString(result.Characters, new List<ReadCharacterDto>(), ref intel);
            var bestSystemMatch = GetSystemFromIntelString(result.Systems, new List<ReadStarSystemDto>(), ref intel);

            _logger.LogInformation($"System: {JsonSerializer.Serialize(bestSystemMatch)}");
            _logger.LogInformation($"Characters: {JsonSerializer.Serialize(bestCharacterMatches)}");
            _logger.LogInformation($"Ships: {JsonSerializer.Serialize(bestShipMatches)}");

            if (!string.IsNullOrWhiteSpace(intel))
            {
                _logger.LogWarning($"Unable to parse remaining intel: '{intel.Trim()}' of '{originalIntel}', candidates: {JsonSerializer.Serialize(candidates)}");
            }

            return new ReportCandidatesDto
            {
                Ships = bestShipMatches,
                StarSystem = bestSystemMatch,
                Characters = bestCharacterMatches,
            };
        }

        private void CreateIntel(ReportCandidatesDto reportCandidates, CreateIntelDto intel)
        {
            var ships = _mapper.Map<IEnumerable<Ship>>(reportCandidates.Ships);
            var characters = _mapper.Map<IEnumerable<Character>>(reportCandidates.Characters);
            var starSystem = _mapper.Map<StarSystem>(reportCandidates.StarSystem);

            if (starSystem != null && !_repository.HasStarSystem(starSystem.StarSystemId))
            {
                _repository.CreateStarSystem(starSystem);
            }

            var characterShipPairs = characters
                .Select((c, i) => new Tuple<Character, Ship>(c, ships.ElementAtOrDefault(i)));

            DateTime.TryParse(intel.Timestamp, out var timestamp);

            var intels = new List<Intel>();
            foreach (var characterAndShip in characterShipPairs)
            {
                var (character, ship) = characterAndShip;

                if (_repository.GetIntelForCharacterWithIdAtTimestamp(character.CharacterId, timestamp) != null)
                {
                    _logger.LogInformation($"Duplicate intel for {character.Name} with ship {ship?.Name ?? "(null)"} in {starSystem.Name}");
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

            _repository.SaveChanges();
        }
    }
}