using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class PlayerInventory
{
    public int PlayerId { get; set; }

    public int ShopProductId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? AcquiredAt { get; set; }

    public virtual User Player { get; set; } = null!;

    public virtual ShopProduct ShopProduct { get; set; } = null!;
}
