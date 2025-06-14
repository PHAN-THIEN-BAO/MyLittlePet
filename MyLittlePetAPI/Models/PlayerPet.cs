using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class PlayerPet
    {
        [Key]
        public int PlayerPetID { get; set; }
        
        public int PlayerID { get; set; }
        
        public int PetID { get; set; }
        
        [StringLength(50)]
        public string PetName { get; set; }
        
        public DateTime AdoptedAt { get; set; } = DateTime.Now;
        
        public int Level { get; set; } = 0;
        
        [StringLength(50)]
        public string Status { get; set; }
        
        public DateTime LastStatusUpdate { get; set; } = DateTime.Now;
        
        // Navigation properties
        [JsonIgnore]
        public virtual User Player { get; set; }
        
        [JsonIgnore]
        public virtual Pet Pet { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<CareHistory> CareHistories { get; set; }
    }
}
