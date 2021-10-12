using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIMP_Server.Models
{
    public class StarSystem
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StarSystemId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ConstellationId { get; set; }

        public ICollection<Intel> Intel { get; set; }

        public IEnumerable<Stargate> OutgoingStargates { get; set; }

        public IEnumerable<Stargate> IncomingStargates { get; set; }
    }
}