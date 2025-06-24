using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyLittlePetGameAPI.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<CareActivity> CareActivities { get; set; }

    public virtual DbSet<CareHistory> CareHistories { get; set; }

    public virtual DbSet<GameRecord> GameRecords { get; set; }

    public virtual DbSet<Minigame> Minigames { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<PlayerAchievement> PlayerAchievements { get; set; }

    public virtual DbSet<PlayerInventory> PlayerInventories { get; set; }

    public virtual DbSet<PlayerPet> PlayerPets { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<ShopProduct> ShopProducts { get; set; }

    public virtual DbSet<User> Users { get; set; }    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string is already configured in Program.cs
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.AchievementId).HasName("PK__Achievem__276330E0590270E6");

            entity.ToTable("Achievement");

            entity.Property(e => e.AchievementId).HasColumnName("AchievementID");
            entity.Property(e => e.AchievementName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
        });

        modelBuilder.Entity<CareActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__CareActi__45F4A7F18083DDA7");

            entity.ToTable("CareActivity");

            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
        });

        modelBuilder.Entity<CareHistory>(entity =>
        {
            entity.HasKey(e => e.CareHistoryId).HasName("PK__CareHist__487B225C2496CE4D");

            entity.ToTable("CareHistory");

            entity.Property(e => e.CareHistoryId).HasColumnName("CareHistoryID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.PerformedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.PlayerPetId).HasColumnName("PlayerPetID");

            entity.HasOne(d => d.Activity).WithMany(p => p.CareHistories)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareHisto__Activ__6D0D32F4");

            entity.HasOne(d => d.Player).WithMany(p => p.CareHistories)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareHisto__Playe__6E01572D");

            entity.HasOne(d => d.PlayerPet).WithMany(p => p.CareHistories)
                .HasForeignKey(d => d.PlayerPetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareHisto__Playe__6C190EBB");
        });

        modelBuilder.Entity<GameRecord>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.MinigameId }).HasName("PK__GameReco__6E56A2B80B2C44FD");

            entity.ToTable("GameRecord");

            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.MinigameId).HasColumnName("MinigameID");
            entity.Property(e => e.PlayedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Minigame).WithMany(p => p.GameRecords)
                .HasForeignKey(d => d.MinigameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameRecor__Minig__7B5B524B");

            entity.HasOne(d => d.Player).WithMany(p => p.GameRecords)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameRecor__Playe__7A672E12");
        });

        modelBuilder.Entity<Minigame>(entity =>
        {
            entity.HasKey(e => e.MinigameId).HasName("PK__Minigame__418D6103DAA3F1C0");

            entity.ToTable("Minigame");

            entity.Property(e => e.MinigameId).HasColumnName("MinigameID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("PK__Pet__48E538029D8E7A99");

            entity.ToTable("Pet");

            entity.Property(e => e.PetId).HasColumnName("PetID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.PetDefaultName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PetType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Admin).WithMany(p => p.Pets)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__Pet__AdminID__5EBF139D");
        });

        modelBuilder.Entity<PlayerAchievement>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.AchievementId }).HasName("PK__PlayerAc__583847A6C54A84F8");

            entity.ToTable("PlayerAchievement");

            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.AchievementId).HasColumnName("AchievementID");
            entity.Property(e => e.EarnedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Achievement).WithMany(p => p.PlayerAchievements)
                .HasForeignKey(d => d.AchievementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerAch__Achie__74AE54BC");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerAchievements)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerAch__Playe__73BA3083");
        });

        modelBuilder.Entity<PlayerInventory>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.ShopProductId }).HasName("PK__PlayerIn__00D1CFDB32E1CA70");

            entity.ToTable("PlayerInventory");

            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.ShopProductId).HasColumnName("ShopProductID");
            entity.Property(e => e.AcquiredAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerInventories)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerInv__Playe__5AEE82B9");

            entity.HasOne(d => d.ShopProduct).WithMany(p => p.PlayerInventories)
                .HasForeignKey(d => d.ShopProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerInv__ShopP__5BE2A6F2");
        });

        modelBuilder.Entity<PlayerPet>(entity =>
        {
            entity.HasKey(e => e.PlayerPetId).HasName("PK__PlayerPe__C0CA3F2A8D27F564");

            entity.ToTable("PlayerPet");

            entity.HasIndex(e => new { e.PlayerId, e.PetCustomName }, "UQ__PlayerPe__5D4E8263679F3BC1").IsUnique();

            entity.Property(e => e.PlayerPetId).HasColumnName("PlayerPetID");
            entity.Property(e => e.AdoptedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastStatusUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Level).HasDefaultValue(1);
            entity.Property(e => e.PetCustomName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PetId).HasColumnName("PetID");
            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Pet).WithMany(p => p.PlayerPets)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerPet__PetID__66603565");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerPets)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerPet__Playe__656C112C");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PK__Shop__67C556292F8438D6");

            entity.ToTable("Shop");

            entity.Property(e => e.ShopId).HasColumnName("ShopID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShopProduct>(entity =>
        {
            entity.HasKey(e => e.ShopProductId).HasName("PK__ShopProd__A9FBB735604081CF");

            entity.ToTable("ShopProduct");

            entity.Property(e => e.ShopProductId).HasColumnName("ShopProductID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.PetId).HasColumnName("PetID");
            entity.Property(e => e.CurrencyType)
                .HasMaxLength(20)
                .IsUnicode(false);            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ShopId).HasColumnName("ShopID");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Admin).WithMany(p => p.ShopProducts)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShopProdu__Admin__5629CD9C");

            entity.HasOne(d => d.Pet).WithMany()
                .HasForeignKey(d => d.PetId)
                .HasConstraintName("FK__ShopProdu__PetId");

            entity.HasOne(d => d.Shop).WithMany(p => p.ShopProducts)
                .HasForeignKey(d => d.ShopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShopProdu__ShopI__5535A963");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC27DE44E432");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534B334BD88").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Diamond).HasDefaultValue(0);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gem).HasDefaultValue(0);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Level).HasDefaultValue(1);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.UserStatus)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
