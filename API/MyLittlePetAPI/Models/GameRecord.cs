using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class GameRecord
    {
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int MinigameID { get; set; }
        
        public DateTime PlayedAt { get; set; } = DateTime.Now;
        
        public int? Score { get; set; }

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual User Player { get; set; } = null!;
        
        [ForeignKey("MinigameID")]
        public virtual Minigame Minigame { get; set; } = null!;
    }
}
