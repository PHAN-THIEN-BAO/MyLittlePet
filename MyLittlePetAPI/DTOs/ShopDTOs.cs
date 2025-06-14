using System.ComponentModel.DataAnnotations;

namespace MyLittlePetAPI.DTOs
{
    public class ShopDTO
    {
        public int ShopID { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public string Description { get; set; }
    }

    public class CreateShopDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(10)]
        public string Type { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
    }

    public class UpdateShopDTO
    {
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(10)]
        public string Type { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
    }

    public class ShopProductDTO
    {
        public int ShopProductID { get; set; }
        
        public int ShopID { get; set; }
        
        public int AdminID { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public string Description { get; set; }
        
        public string ImageUrl { get; set; }
        
        public int Price { get; set; }
        
        public string CurrencyType { get; set; }
        
        public int Quality { get; set; }
        
        public string ShopName { get; set; }
        
        public string AdminName { get; set; }
    }

    public class CreateShopProductDTO
    {
        [Required]
        public int ShopID { get; set; }
        
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
    }

    public class UpdateShopProductDTO
    {
        public int? ShopID { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(20)]
        public string Type { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
        
        [StringLength(255)]
        public string ImageUrl { get; set; }
        
        public int? Price { get; set; }
        
        [StringLength(20)]
        public string CurrencyType { get; set; }
        
        public int? Quality { get; set; }
    }

    public class PlayerInventoryDTO
    {
        public int PlayerID { get; set; }
        
        public int ShopProductID { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime AcquiredAt { get; set; }
        
        public string PlayerName { get; set; }
        
        public ShopProductDTO ShopProduct { get; set; }
    }

    public class CreatePlayerInventoryDTO
    {
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int ShopProductID { get; set; }
        
        public int Quantity { get; set; } = 1;
    }

    public class UpdatePlayerInventoryDTO
    {
        public int Quantity { get; set; }
    }

    public class PurchaseItemDTO
    {
        [Required]
        public int ShopProductID { get; set; }
        
        public int Quantity { get; set; } = 1;
    }
}
