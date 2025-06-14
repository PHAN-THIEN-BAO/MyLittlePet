using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class Achievement
    {
        [Key]
        public int AchievementID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AchievementName { get; set; }
        
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; }
    }
}
