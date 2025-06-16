using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class Shop
{
    public int ShopId { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ShopProduct> ShopProducts { get; set; } = new List<ShopProduct>();
}
