using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class PlayerPet
    {
        [Key]
        public int PlayerPetID { get; set; }
        
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int PetID { get; set; }
        
        [StringLength(50)]
        public string? PetName { get; set; }
        
        public DateTime AdoptedAt { get; set; } = DateTime.Now;
        
        public int Level { get; set; } = 0;
        
        [StringLength(50)]
        public string? Status { get; set; }
        
        public DateTime LastStatusUpdate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual User Player { get; set; } = null!;
        
        [ForeignKey("PetID")]
        public virtual Pet Pet { get; set; } = null!;
        
        public virtual ICollection<CareHistory> CareHistories { get; set; } = new List<CareHistory>();
    }
}
