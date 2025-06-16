using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class Achievement
{
    public int AchievementId { get; set; }

    public string AchievementName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
}
