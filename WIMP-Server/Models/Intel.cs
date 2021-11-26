using System;
using System.ComponentModel.DataAnnotations;

namespace WIMP_Server.Models;

public class Intel
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    public int StarSystemId { get; set; }

    public int? CharacterId { get; set; }

    public int? ShipId { get; set; }

    public int? ReportedById { get; set; }

    public StarSystem StarSystem { get; set; }

    public Character Character { get; set; }
    public Ship Ship { get; set; }
    public Character ReportedBy { get; set; }

    public bool IsSpike { get; set; }

    public bool IsClear { get; set; }
}
