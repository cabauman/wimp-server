using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi;

public class EsiUniverseBulkSearchResponseDto
{
    public static readonly EsiUniverseBulkSearchResponseDto Empty = new()
    {
        Agents = Array.Empty<EsiNameIdPairDto>(),
        Alliances = Array.Empty<EsiNameIdPairDto>(),
        Characters = Array.Empty<EsiSearchCharacterDto>(),
        Constellations = Array.Empty<EsiNameIdPairDto>(),
        Corporations = Array.Empty<EsiNameIdPairDto>(),
        Factions = Array.Empty<EsiNameIdPairDto>(),
        InventoryTypes = Array.Empty<EsiNameIdPairDto>(),
        Regions = Array.Empty<EsiNameIdPairDto>(),
        Stations = Array.Empty<EsiNameIdPairDto>(),
        Systems = Array.Empty<EsiSearchSystemDto>(),
    };

    [JsonPropertyName("agents")]
    public IEnumerable<EsiNameIdPairDto> Agents { get; set; }

    [JsonPropertyName("alliances")]
    public IEnumerable<EsiNameIdPairDto> Alliances { get; set; }

    [JsonPropertyName("characters")]
    public IEnumerable<EsiSearchCharacterDto> Characters { get; set; }

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
    public IEnumerable<EsiSearchSystemDto> Systems { get; set; }
}
