using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [JsonIgnore]
        public string Password { get; set; }

        public int Level { get; set; } = 1;
        
        public int Coin { get; set; }
        
        public int Diamond { get; set; } = 0;
        
        public int Gem { get; set; } = 0;
        
        public DateTime JoinDate { get; set; } = DateTime.Now;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<ShopProduct> ShopProducts { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<PlayerInventory> PlayerInventories { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Pet> Pets { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<PlayerPet> PlayerPets { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<CareHistory> CareHistories { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<GameRecord> GameRecords { get; set; }
    }
}
