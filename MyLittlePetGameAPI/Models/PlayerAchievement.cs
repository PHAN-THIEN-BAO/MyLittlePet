using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class PlayerAchievement
{
    public int PlayerId { get; set; }

    public int AchievementId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public virtual Achievement Achievement { get; set; } = null!;

    public virtual User Player { get; set; } = null!;
}
