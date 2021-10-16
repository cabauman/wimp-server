using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIMP_Server.Models
{
    public class Stargate
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StargateId { get; set; }

        public int? SrcStarSystemId { get; set; }

        public int? DstStarSystemId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Intel> Intel { get; set; }

        public StarSystem SrcStarSystem { get; set; }

        public StarSystem DstStarSystem { get; set; }
    }
}