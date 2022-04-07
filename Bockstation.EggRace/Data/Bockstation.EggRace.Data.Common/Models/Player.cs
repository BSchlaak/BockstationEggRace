using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bockstation.EggRace.Data.Common.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; }

        public string Name { get; set; }
    }
}
