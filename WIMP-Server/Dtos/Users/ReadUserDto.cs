using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users;

public class ReadUserDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("roles")]
    public IEnumerable<string> Roles { get; set; }
}
