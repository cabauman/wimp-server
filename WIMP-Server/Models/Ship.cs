using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIMP_Server.Models;

public class Ship
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ShipId { get; set; }

    [Required]
    public string Name { get; set; }

    [NotMapped]
    public ICollection<Intel> Intel { get; set; }
}
