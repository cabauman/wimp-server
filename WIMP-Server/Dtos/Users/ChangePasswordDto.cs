using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class ChangePasswordDto
    {
        [Required]
        [JsonPropertyName("currentPassword")]
        public string CurrentPassword { get; set; }

        [Required]
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
    }
}