using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class PlayerInventory
    {
        [Key]
        [Column(Order = 0)]
        public int PlayerID { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int ShopProductID { get; set; }
        
        public int Quantity { get; set; } = 1;
        
        public DateTime AcquiredAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [JsonIgnore]
        public virtual User Player { get; set; }
        
        [JsonIgnore]
        public virtual ShopProduct ShopProduct { get; set; }
    }
}
