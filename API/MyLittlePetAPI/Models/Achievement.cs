using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class Achievement
    {
        [Key]
        public int AchievementID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AchievementName { get; set; } = string.Empty;
        
        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}
