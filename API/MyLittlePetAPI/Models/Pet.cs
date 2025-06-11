using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class Pet
    {
        [Key]
        public int PetID { get; set; }
        
        public int? AdminID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PetType { get; set; } = string.Empty;
        
        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }

        // Navigation properties
        [ForeignKey("AdminID")]
        public virtual User? Admin { get; set; }
        
        public virtual ICollection<PlayerPet> PlayerPets { get; set; } = new List<PlayerPet>();
    }
}
