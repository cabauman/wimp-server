using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi
{
    public class EsiUniverseBulkSearchResponseDto
    {
        [JsonPropertyName("agents")]
        public IEnumerable<EsiNameIdPairDto> Agents { get; set; }
        [JsonPropertyName("alliances")]
        public IEnumerable<EsiNameIdPairDto> Alliances { get; set; }
        [JsonPropertyName("characters")]
        public IEnumerable<EsiNameIdPairDto> Characters { get; set; }
        [JsonPropertyName("constellations")]
        public IEnumerable<EsiNameIdPairDto> Constellations { get; set; }
        [JsonPropertyName("corporations")]
        public IEnumerable<EsiNameIdPairDto> Corporations { get; set; }
        [JsonPropertyName("factions")]
        public IEnumerable<EsiNameIdPairDto> Factions { get; set; }

        [JsonPropertyName("inventory_types")]
        public IEnumerable<EsiNameIdPairDto> InventoryTypes { get; set; }
        [JsonPropertyName("regions")]
        public IEnumerable<EsiNameIdPairDto> Regions { get; set; }
        [JsonPropertyName("stations")]
        public IEnumerable<EsiNameIdPairDto> Stations { get; set; }
        [JsonPropertyName("systems")]
        public IEnumerable<EsiNameIdPairDto> Systems { get; set; }
    }
}