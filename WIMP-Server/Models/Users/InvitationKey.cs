using System;
using System.ComponentModel.DataAnnotations;

namespace WIMP_Server.Models.Users;

public class InvitationKey
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Key { get; set; }

    [Required]
    public string GeneratedByUserId { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    public User GeneratedByUser { get; set; }
}
