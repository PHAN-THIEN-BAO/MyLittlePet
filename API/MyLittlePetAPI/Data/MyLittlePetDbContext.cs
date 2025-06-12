using Microsoft.EntityFrameworkCore;
using MyLittlePetAPI.Models;

namespace MyLittlePetAPI.Data
{
    public class MyLittlePetDbContext : DbContext
    {
        public MyLittlePetDbContext(DbContextOptions<MyLittlePetDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopProduct> ShopProducts { get; set; }
        public DbSet<PlayerInventory> PlayerInventories { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PlayerPet> PlayerPets { get; set; }
        public DbSet<CareActivity> CareActivities { get; set; }
        public DbSet<CareHistory> CareHistories { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<PlayerAchievement> PlayerAchievements { get; set; }
        public DbSet<Minigame> Minigames { get; set; }
        public DbSet<GameRecord> GameRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary keys
            modelBuilder.Entity<PlayerInventory>()
                .HasKey(pi => new { pi.PlayerID, pi.ShopProductID });

            modelBuilder.Entity<PlayerAchievement>()
                .HasKey(pa => new { pa.PlayerID, pa.AchievementID });

            modelBuilder.Entity<GameRecord>()
                .HasKey(gr => new { gr.PlayerID, gr.MinigameID });

            // Configure unique constraint for PlayerPet
            modelBuilder.Entity<PlayerPet>()
                .HasIndex(pp => new { pp.PlayerID, pp.PetName })
                .IsUnique();

            // Configure User email unique constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure relationships that might need explicit configuration
            modelBuilder.Entity<ShopProduct>()
                .HasOne(sp => sp.Shop)
                .WithMany(s => s.ShopProducts)
                .HasForeignKey(sp => sp.ShopID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopProduct>()
                .HasOne(sp => sp.Admin)
                .WithMany(u => u.ShopProducts)
                .HasForeignKey(sp => sp.AdminID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerInventory>()
                .HasOne(pi => pi.Player)
                .WithMany(u => u.PlayerInventories)
                .HasForeignKey(pi => pi.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerInventory>()
                .HasOne(pi => pi.ShopProduct)
                .WithMany(sp => sp.PlayerInventories)
                .HasForeignKey(pi => pi.ShopProductID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Admin)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.AdminID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PlayerPet>()
                .HasOne(pp => pp.Player)
                .WithMany(u => u.PlayerPets)
                .HasForeignKey(pp => pp.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerPet>()
                .HasOne(pp => pp.Pet)
                .WithMany(p => p.PlayerPets)
                .HasForeignKey(pp => pp.PetID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CareHistory>()
                .HasOne(ch => ch.PlayerPet)
                .WithMany(pp => pp.CareHistories)
                .HasForeignKey(ch => ch.PlayerPetID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CareHistory>()
                .HasOne(ch => ch.Player)
                .WithMany(u => u.CareHistories)
                .HasForeignKey(ch => ch.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CareHistory>()
                .HasOne(ch => ch.CareActivity)
                .WithMany(ca => ca.CareHistories)
                .HasForeignKey(ch => ch.ActivityID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAchievement>()
                .HasOne(pa => pa.Player)
                .WithMany(u => u.PlayerAchievements)
                .HasForeignKey(pa => pa.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAchievement>()
                .HasOne(pa => pa.Achievement)
                .WithMany(a => a.PlayerAchievements)
                .HasForeignKey(pa => pa.AchievementID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameRecord>()
                .HasOne(gr => gr.Player)
                .WithMany(u => u.GameRecords)
                .HasForeignKey(gr => gr.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameRecord>()
                .HasOne(gr => gr.Minigame)
                .WithMany(m => m.GameRecords)
                .HasForeignKey(gr => gr.MinigameID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
