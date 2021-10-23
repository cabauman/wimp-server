using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class ReadTokenDto
    {
        [Required]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [Required]
        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }
    }
}