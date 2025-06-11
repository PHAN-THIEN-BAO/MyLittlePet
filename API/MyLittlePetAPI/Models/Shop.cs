using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class Shop
    {
        [Key]
        public int ShopID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string? Type { get; set; }
        
        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<ShopProduct> ShopProducts { get; set; } = new List<ShopProduct>();
    }
}
