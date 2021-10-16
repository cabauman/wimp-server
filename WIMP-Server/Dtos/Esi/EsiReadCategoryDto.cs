using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi
{
    public class EsiReadCategoryDto
    {
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("groups")]
        public IEnumerable<int> Groups { get; set; }
    }
}