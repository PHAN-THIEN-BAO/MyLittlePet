using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittlePetAPI.Models
{
    public class PlayerInventory
    {
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int ShopProductID { get; set; }
        
        public int Quantity { get; set; } = 1;
        
        public DateTime AcquiredAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual User Player { get; set; } = null!;
        
        [ForeignKey("ShopProductID")]
        public virtual ShopProduct ShopProduct { get; set; } = null!;
    }
}
