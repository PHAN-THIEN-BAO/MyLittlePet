using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class Minigame
    {
        [Key]
        public int MinigameID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<GameRecord> GameRecords { get; set; }
    }
}
