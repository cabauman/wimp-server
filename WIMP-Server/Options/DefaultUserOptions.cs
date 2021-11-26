using System.ComponentModel.DataAnnotations;

namespace WIMP_Server.Options;

public class DefaultUserOptions
{
    public static readonly string Key = "DefaultUser";

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public bool DisableCreate { get; set; }
}
