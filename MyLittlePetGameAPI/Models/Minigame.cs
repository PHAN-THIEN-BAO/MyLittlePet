using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class Minigame
{
    public int MinigameId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();
}
