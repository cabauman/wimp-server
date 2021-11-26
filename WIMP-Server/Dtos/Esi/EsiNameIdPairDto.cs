using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi;

public class EsiNameIdPairDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
