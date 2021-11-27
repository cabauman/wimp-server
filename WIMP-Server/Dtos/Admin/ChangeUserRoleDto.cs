using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Admin;

public class ChangeUserRoleDto
{
    [Required]
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [Required]
    [JsonPropertyName("new_role")]
    public string NewRole { get; set; }
}
