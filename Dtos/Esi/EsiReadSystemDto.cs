using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi
{
    public class EsiReadSystemDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("system_id")]
        public int SystemId { get; set; }

        [JsonPropertyName("constellation_id")]
        public int ConstellationId { get; set; }

        [JsonPropertyName("stargates")]
        public IEnumerable<int> Stargates { get; set; }
    }
}