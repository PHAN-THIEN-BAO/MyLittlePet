using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class ShopProduct
    {
        [Key]
        public int ShopProductID { get; set; }

        public int ShopID { get; set; }
        
        public int AdminID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        [StringLength(20)]
        public string CurrencyType { get; set; }

        public int Quality { get; set; } = 100;

        // Navigation properties
        [JsonIgnore]
        public virtual Shop Shop { get; set; }
        
        [JsonIgnore]
        public virtual User Admin { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<PlayerInventory> PlayerInventories { get; set; }
    }
}
