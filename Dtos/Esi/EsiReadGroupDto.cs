using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi
{
    public class EsiReadGroupDto
    {
        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }

        [JsonPropertyName("types")]
        public IEnumerable<int> Types { get; set; }
    }
}