using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WIMP_Server.Data;
using WIMP_Server.Dtos.Universe;
using WIMP_Server.Models;
using WIMP_Server.Searching;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UniverseController : ControllerBase
    {
        private readonly ILogger<UniverseController> _logger;
        private readonly IWimpRepository _repository;

        public UniverseController(ILogger<UniverseController> logger, IWimpRepository repository)
        {
            _logger = logger;
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
        public ActionResult<UniverseReadGraphDto> GetGraphForSystemsWithinJumps(int systemId, int jumps)
        {
            //var visitedSystems = new Dictionary<int, StarSystem>();

            var system = GetStarsystemWithGates(systemId);
            if (system == null) return NotFound();

            var systemsWithinJumps = GraphTraversalUtilities
                .BreadthFirstSearchWithinDistance(system, jumps,
                n => n.StarSystemId,
                n => GetStarsystemWithGates(n.StarSystemId).OutgoingStargates
                    .Select(sg => _repository.GetStarSystemWithId(sg.DstStarSystemId.Value)));

            // TraverseSystems(visitedSystems, system, jumps, 0);

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

        private void TraverseSystems(Dictionary<int, StarSystem> visitedSystems, StarSystem system, int jumps, int depth)
        {
            if (depth > jumps) return;
            if (visitedSystems.ContainsKey(system.StarSystemId)) return;

            visitedSystems.Add(system.StarSystemId, system);

            var gates = system.OutgoingStargates;

            foreach (var gate in gates)
            {
                var nextSystem = GetStarsystemWithGates(gate.DstStarSystemId.Value);
                TraverseSystems(visitedSystems, nextSystem, jumps, depth + 1);
            }
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