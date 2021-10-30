using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIMP_Server.Auth.Policies;
using WIMP_Server.Data;
using WIMP_Server.Dtos.Universe;
using WIMP_Server.Models;
using WIMP_Server.Searching;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = Policy.OnlyUsers)]
    public class UniverseController : ControllerBase
    {
        private readonly IWimpRepository _repository;

        public UniverseController(IWimpRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<UniverseReadGraphDto> GetFullGraph()
        {
            var starSystems = _repository.GetAllStarSystems();
            var stargates = _repository.GetAllStargates();

            var result = new UniverseReadGraphDto
            {
                Systems = starSystems
                    .Select(s => new Node
                    {
                        SystemId = s.StarSystemId,
                        SystemName = s.Name
                    }),
                Edges = stargates
                    .Where(s => s.DstStarSystemId != null && s.SrcStarSystemId != null)
                    .Select(s => new Edge
                    {
                        SourceSystemId = s.SrcStarSystemId ?? 0,
                        DestinationSystemId = s.DstStarSystemId ?? 0
                    }),
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("{systemId}/{jumps}")]
        [ProducesResponseType(typeof(UniverseReadGraphDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UniverseReadGraphDto> GetGraphForSystemsWithinJumps(int systemId, int jumps)
        {
            var system = GetStarsystemWithGates(systemId);
            if (system == null) return NotFound();

            var systemsWithinJumps = GraphTraversalUtilities
                .BreadthFirstSearchWithinDistance(system, jumps,
                n => n.StarSystemId,
                n => GetStarsystemWithGates(n.StarSystemId).OutgoingStargates
                    .Select(sg => _repository.GetStarSystemWithId(sg.DstStarSystemId.Value)));

            var edges = new List<Edge>();
            foreach (var visitedSystem in systemsWithinJumps)
            {
                var systemEdges = visitedSystem.OutgoingStargates
                    .Where(sg => systemsWithinJumps
                        .Any(s =>
                            s.StarSystemId == sg.DstStarSystemId) &&
                            !edges.Any(e =>
                                sg.SrcStarSystemId == e.DestinationSystemId &&
                                sg.DstStarSystemId == e.SourceSystemId))
                    .Select(sg => new Edge
                    {
                        SourceSystemId = sg.SrcStarSystemId.Value,
                        DestinationSystemId = sg.DstStarSystemId.Value
                    });

                edges.AddRange(systemEdges);
            }

            var result = new UniverseReadGraphDto
            {
                Systems = systemsWithinJumps
                    .Select(s => new Node
                    {
                        SystemId = s.StarSystemId,
                        SystemName = s.Name
                    }),
                Edges = edges
            };

            return Ok(result);
        }

        private StarSystem GetStarsystemWithGates(int id)
        {
            var system = _repository.GetStarSystemWithId(id);
            system.IncomingStargates = _repository.GetStargatesWithDestinationSystemId(id);
            system.OutgoingStargates = _repository.GetStargatesWithSourceSystemId(id);

            return system;
        }
    }
}