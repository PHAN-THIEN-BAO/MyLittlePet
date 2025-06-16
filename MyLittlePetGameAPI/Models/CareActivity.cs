using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class CareActivity
{
    public int ActivityId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<CareHistory> CareHistories { get; set; } = new List<CareHistory>();
}
