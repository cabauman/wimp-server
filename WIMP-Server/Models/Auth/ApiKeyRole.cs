using System.ComponentModel.DataAnnotations;

namespace WIMP_Server.Models.Auth
{
    public class ApiKeyRole
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string Role { get; set; }

        public ApiKey Owner { get; set; }
    }
}