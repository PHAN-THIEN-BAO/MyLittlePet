using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? UserName { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
        
        public int Level { get; set; } = 1;
        
        public int? Coin { get; set; }
        
        public int Diamond { get; set; } = 0;
        
        public int Gem { get; set; } = 0;
        
        public DateTime JoinDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ShopProduct> ShopProducts { get; set; } = new List<ShopProduct>();
        public virtual ICollection<PlayerInventory> PlayerInventories { get; set; } = new List<PlayerInventory>();
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public virtual ICollection<PlayerPet> PlayerPets { get; set; } = new List<PlayerPet>();
        public virtual ICollection<CareHistory> CareHistories { get; set; } = new List<CareHistory>();
        public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
        public virtual ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();
    }
}
