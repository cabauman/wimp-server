using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WIMP_Server.Dtos.Esi
{
    /// <summary>
    /// Data used to report intel.
    /// </summary>
    public class CreateIntelDto
    {
        /// <summary>
        /// The intel message text string.
        /// </summary>
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// The name of player who reported the intel.
        /// </summary>
        [JsonPropertyName("reportedBy")]
        public string ReportedBy { get; set; }

        /// <summary>
        /// The timestamp of the intel message.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}