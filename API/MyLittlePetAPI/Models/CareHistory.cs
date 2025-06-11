using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class CareHistory
    {
        [Key]
        public int CareHistoryID { get; set; }
        
        [Required]
        public int PlayerPetID { get; set; }
        
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int ActivityID { get; set; }
        
        public DateTime PerformedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PlayerPetID")]
        public virtual PlayerPet PlayerPet { get; set; } = null!;
        
        [ForeignKey("PlayerID")]
        public virtual User Player { get; set; } = null!;
        
        [ForeignKey("ActivityID")]
        public virtual CareActivity CareActivity { get; set; } = null!;
    }
}
