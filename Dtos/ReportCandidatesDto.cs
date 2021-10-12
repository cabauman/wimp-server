using System.Collections.Generic;

namespace WIMP_Server.Dtos.Esi
{
    public class ReportCandidatesDto
    {
        public IEnumerable<ReadCharacterDto> Characters { get; set; }

        public IEnumerable<ReadShipDto> Ships { get; set; }

        public ReadStarSystemDto StarSystem { get; set; }
    }
}