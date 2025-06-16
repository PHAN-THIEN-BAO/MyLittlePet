using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Role { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? UserStatus { get; set; }

    public int? Level { get; set; }

    public int? Coin { get; set; }

    public int? Diamond { get; set; }

    public int? Gem { get; set; }

    public DateTime? JoinDate { get; set; }

    public virtual ICollection<CareHistory> CareHistories { get; set; } = new List<CareHistory>();

    public virtual ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();

    public virtual ICollection<PlayerInventory> PlayerInventories { get; set; } = new List<PlayerInventory>();

    public virtual ICollection<PlayerPet> PlayerPets { get; set; } = new List<PlayerPet>();

    public virtual ICollection<ShopProduct> ShopProducts { get; set; } = new List<ShopProduct>();
}
