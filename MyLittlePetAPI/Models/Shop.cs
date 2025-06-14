using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class Shop
    {
        [Key]
        public int ShopID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Type { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<ShopProduct> ShopProducts { get; set; }
    }
}
