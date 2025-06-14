using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class Pet
    {
        [Key]
        public int PetID { get; set; }
        
        public int? AdminID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PetType { get; set; }
        
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual User Admin { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<PlayerPet> PlayerPets { get; set; }
    }
}
