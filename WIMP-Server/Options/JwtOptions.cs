using System.ComponentModel.DataAnnotations;

namespace WIMP_Server.Options
{
    public class JwtOptions
    {
        public static readonly string Key = "JWT";

        [Required]
        public string ValidAudience { get; set; }

        [Required]
        public string ValidIssuer { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        public int ExpiresAfterMinutes { get; set; }
    }
}