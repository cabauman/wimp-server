using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIMP_Server.Dtos.Esi;

namespace WIMP_Server.Dtos.Picture
{
    public class ReadPictureDto
    {
        [JsonPropertyName("since_time")]
        public DateTime SinceTime { get; set; }

        [JsonPropertyName("generated_time")]
        public DateTime GeneratedTime { get; set; }

        [JsonPropertyName("reported_intel")]
        public IEnumerable<ReadIntelDto> ReportedIntel { get; set; }

        [JsonPropertyName("reported_characters")]
        public IEnumerable<int> ReportedCharacters { get; set; }

        [JsonPropertyName("reported_ships")]
        public IEnumerable<int> ReportedShips { get; set; }

        [JsonPropertyName("reported_systems")]
        public IEnumerable<int> ReportedSystems { get; set; }
    }
}