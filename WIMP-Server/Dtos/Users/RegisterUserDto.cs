using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class RegisterUserDto
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("invite_key")]
        public string InviteKey { get; set; }
    }
}