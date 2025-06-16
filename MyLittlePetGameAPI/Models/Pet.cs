using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class Pet
{
    public int PetId { get; set; }

    public int? AdminId { get; set; }

    public string PetType { get; set; } = null!;

    public string PetDefaultName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual User? Admin { get; set; }

    public virtual ICollection<PlayerPet> PlayerPets { get; set; } = new List<PlayerPet>();
}
