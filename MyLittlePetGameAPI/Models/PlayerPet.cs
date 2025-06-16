using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class PlayerPet
{
    public int PlayerPetId { get; set; }

    public int PlayerId { get; set; }

    public int PetId { get; set; }

    public string? PetCustomName { get; set; }

    public DateTime? AdoptedAt { get; set; }

    public int? Level { get; set; }

    public string? Status { get; set; }

    public DateTime? LastStatusUpdate { get; set; }

    public virtual ICollection<CareHistory> CareHistories { get; set; } = new List<CareHistory>();

    public virtual Pet Pet { get; set; } = null!;

    public virtual User Player { get; set; } = null!;
}
