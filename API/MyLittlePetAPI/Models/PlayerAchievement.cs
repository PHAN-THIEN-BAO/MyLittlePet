using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class PlayerAchievement
    {
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int AchievementID { get; set; }
        
        public DateTime EarnedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual User Player { get; set; } = null!;
        
        [ForeignKey("AchievementID")]
        public virtual Achievement Achievement { get; set; } = null!;
    }
}
