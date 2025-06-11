using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class CareActivity
    {
        [Key]
        public int ActivityID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; } = string.Empty;
        
        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<CareHistory> CareHistories { get; set; } = new List<CareHistory>();
    }
}
