using System;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class ReadInvitationKeyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("generated_by")]
        public string GeneratedByUserId { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}