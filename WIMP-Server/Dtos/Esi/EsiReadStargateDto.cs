using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi
{
    public class StargateDestination
    {
        [JsonPropertyName("stargate_id")]
        public int StargateId { get; set; }

        [JsonPropertyName("system_id")]
        public int SystemId { get; set; }
    }

    public class EsiReadStargateDto
    {
        [JsonPropertyName("stargate_id")]
        public int StargateId { get; set; }

        [JsonPropertyName("system_id")]
        public int SrcSystemId { get; set; }

        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("destination")]
        public StargateDestination Destination { get; set; }
    }
}

/* {
  "destination": {
    "stargate_id": 50000953,
    "system_id": 30001190
  },
  "name": "Stargate (JWZ2-V)",
  "position": {
    "x": 1577982566400.0,
    "y": 181942640640.0,
    "z": 946528051200.0
  },
  "stargate_id": 50001898,
  "system_id": 30001192,
  "type_id": 29624
}
*/