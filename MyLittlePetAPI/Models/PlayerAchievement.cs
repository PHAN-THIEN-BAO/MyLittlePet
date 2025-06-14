using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class PlayerAchievement
    {
        [Key]
        [Column(Order = 0)]
        public int PlayerID { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int AchievementID { get; set; }
        
        public DateTime EarnedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [JsonIgnore]
        public virtual User Player { get; set; }
        
        [JsonIgnore]
        public virtual Achievement Achievement { get; set; }
    }
}
