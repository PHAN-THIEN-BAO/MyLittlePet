using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class ShopProduct
    {
        [Key]
        public int ShopProductID { get; set; }
        
        [Required]
        public int ShopID { get; set; }
        
        [Required]
        public int AdminID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Description { get; set; }
        
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        
        [Required]
        public int Price { get; set; }
        
        [Required]
        [StringLength(20)]
        public string CurrencyType { get; set; } = string.Empty;
        
        public int Quality { get; set; } = 100;

        // Navigation properties
        [ForeignKey("ShopID")]
        public virtual Shop Shop { get; set; } = null!;
        
        [ForeignKey("AdminID")]
        public virtual User Admin { get; set; } = null!;
        
        public virtual ICollection<PlayerInventory> PlayerInventories { get; set; } = new List<PlayerInventory>();
    }
}
