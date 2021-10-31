using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class ReadApiKeyDto
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<string> Roles { get; set; }
    }
}