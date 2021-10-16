using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WIMP_Server.Data;
using WIMP_Server.Dtos;
using WIMP_Server.Dtos.Picture;
using WIMP_Server.Models;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PictureController : ControllerBase
    {
        private readonly IWimpRepository _repository;
        private readonly IMapper _mapper;

        public PictureController(IWimpRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<ReadPictureDto> GetPicture()
        {
            var currentTime = DateTime.UtcNow;
            var collectReportsSinceTime = currentTime - TimeSpan.FromMinutes(10);

            var allReportsSinceTime = _repository.GetIntelSinceTime(collectReportsSinceTime)
                .Select(i => AssociateIntelReport(i));
            var allCharacters = allReportsSinceTime.Select(i => i.Character.Id).Distinct();
            var allShips = allReportsSinceTime
                .Where(i => i.Ship != null)
                .Select(i => i.Ship.Id)
                .Distinct();

            var allSystems = allReportsSinceTime.Select(i => i.StarSystem.Id).Distinct();

            var picture = new ReadPictureDto
            {
                SinceTime = collectReportsSinceTime,
                GeneratedTime = currentTime,
                ReportedIntel = allReportsSinceTime,
                ReportedCharacters = allCharacters,
                ReportedShips = allShips,
                ReportedSystems = allSystems
            };

            return Ok(picture);
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
    }
}