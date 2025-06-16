using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class GameRecord
{
    public int PlayerId { get; set; }

    public int MinigameId { get; set; }

    public DateTime? PlayedAt { get; set; }

    public int? Score { get; set; }

    public virtual Minigame Minigame { get; set; } = null!;

    public virtual User Player { get; set; } = null!;
}
