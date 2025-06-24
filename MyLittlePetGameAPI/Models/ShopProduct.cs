using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class ShopProduct
{
    public int ShopProductId { get; set; }

    public int ShopId { get; set; }

    public int AdminId { get; set; }

    public int? PetId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }    public int Price { get; set; }    public string CurrencyType { get; set; } = null!;

    public int? Status { get; set; }

    public virtual User Admin { get; set; } = null!;

    public virtual ICollection<PlayerInventory> PlayerInventories { get; set; } = new List<PlayerInventory>();

    public virtual Shop Shop { get; set; } = null!;
    
    public virtual Pet? Pet { get; set; }
}
