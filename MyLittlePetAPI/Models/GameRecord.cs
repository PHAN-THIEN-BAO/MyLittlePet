using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class GameRecord
    {
        [Key]
        [Column(Order = 0)]
        public int PlayerID { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int MinigameID { get; set; }
        
        public DateTime PlayedAt { get; set; } = DateTime.Now;
        
        public int Score { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual User Player { get; set; }
        
        [JsonIgnore]
        public virtual Minigame Minigame { get; set; }
    }
}
