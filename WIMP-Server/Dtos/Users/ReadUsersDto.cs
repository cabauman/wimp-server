using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Users
{
    public class ReadUsersDto
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("users")]
        public IEnumerable<ReadUserDto> Users { get; set; }
    }
}