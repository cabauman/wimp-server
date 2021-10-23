using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class LoginUserDto
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}