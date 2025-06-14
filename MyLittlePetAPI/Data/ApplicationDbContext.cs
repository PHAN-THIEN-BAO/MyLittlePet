using Microsoft.EntityFrameworkCore;
using MyLittlePetAPI.Models;

namespace MyLittlePetAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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

            // Configure PlayerInventory composite key
            modelBuilder.Entity<PlayerInventory>()
                .HasKey(pi => new { pi.PlayerID, pi.ShopProductID });

            // Configure PlayerAchievement composite key
            modelBuilder.Entity<PlayerAchievement>()
                .HasKey(pa => new { pa.PlayerID, pa.AchievementID });

            // Configure GameRecord composite key
            modelBuilder.Entity<GameRecord>()
                .HasKey(gr => new { gr.PlayerID, gr.MinigameID });

            // Configure PlayerPet unique constraint
            modelBuilder.Entity<PlayerPet>()
                .HasIndex(pp => new { pp.PlayerID, pp.PetName })
                .IsUnique();

            // Configure relationships
            modelBuilder.Entity<ShopProduct>()
                .HasOne(sp => sp.Shop)
                .WithMany(s => s.ShopProducts)
                .HasForeignKey(sp => sp.ShopID);

            modelBuilder.Entity<ShopProduct>()
                .HasOne(sp => sp.Admin)
                .WithMany(u => u.ShopProducts)
                .HasForeignKey(sp => sp.AdminID);

            modelBuilder.Entity<PlayerInventory>()
                .HasOne(pi => pi.Player)
                .WithMany(u => u.PlayerInventories)
                .HasForeignKey(pi => pi.PlayerID);

            modelBuilder.Entity<PlayerInventory>()
                .HasOne(pi => pi.ShopProduct)
                .WithMany(sp => sp.PlayerInventories)
                .HasForeignKey(pi => pi.ShopProductID);

            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Admin)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.AdminID)
                .IsRequired(false);

            modelBuilder.Entity<PlayerPet>()
                .HasOne(pp => pp.Player)
                .WithMany(u => u.PlayerPets)
                .HasForeignKey(pp => pp.PlayerID);

            modelBuilder.Entity<PlayerPet>()
                .HasOne(pp => pp.Pet)
                .WithMany(p => p.PlayerPets)
                .HasForeignKey(pp => pp.PetID);

            modelBuilder.Entity<CareHistory>()
                .HasOne(ch => ch.PlayerPet)
                .WithMany(pp => pp.CareHistories)
                .HasForeignKey(ch => ch.PlayerPetID);

            modelBuilder.Entity<CareHistory>()
                .HasOne(ch => ch.Player)
                .WithMany(u => u.CareHistories)
                .HasForeignKey(ch => ch.PlayerID);

            modelBuilder.Entity<CareHistory>()
                .HasOne(ch => ch.Activity)
                .WithMany(ca => ca.CareHistories)
                .HasForeignKey(ch => ch.ActivityID);

            modelBuilder.Entity<PlayerAchievement>()
                .HasOne(pa => pa.Player)
                .WithMany(u => u.PlayerAchievements)
                .HasForeignKey(pa => pa.PlayerID);

            modelBuilder.Entity<PlayerAchievement>()
                .HasOne(pa => pa.Achievement)
                .WithMany(a => a.PlayerAchievements)
                .HasForeignKey(pa => pa.AchievementID);

            modelBuilder.Entity<GameRecord>()
                .HasOne(gr => gr.Player)
                .WithMany(u => u.GameRecords)
                .HasForeignKey(gr => gr.PlayerID);

            modelBuilder.Entity<GameRecord>()
                .HasOne(gr => gr.Minigame)
                .WithMany(m => m.GameRecords)
                .HasForeignKey(gr => gr.MinigameID);
        }
    }
}
