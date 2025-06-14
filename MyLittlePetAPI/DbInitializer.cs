using Microsoft.EntityFrameworkCore;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.Models;

namespace MyLittlePetAPI
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Create the database if it doesn't exist
                context.Database.EnsureCreated();

                // Look for any users
                if (context.Users.Any())
                {
                    return; // DB has been seeded
                }

                // Create admin user
                var adminUser = new User
                {
                    Role = "Admin",
                    UserName = "Admin",
                    Email = "admin@mylittlepet.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Coin = 9999,
                    Diamond = 9999,
                    Gem = 9999,
                    Level = 10,
                    JoinDate = DateTime.Now
                };

                context.Users.Add(adminUser);
                context.SaveChanges();

                // Create shops
                var shops = new List<Shop>
                {
                    new Shop { Name = "Pet Shop", Type = "Pet", Description = "Shop for adopting pets" },
                    new Shop { Name = "Food Shop", Type = "Food", Description = "Shop for pet food" },
                    new Shop { Name = "Toy Shop", Type = "Toy", Description = "Shop for pet toys" },
                    new Shop { Name = "Accessory Shop", Type = "Accessory", Description = "Shop for pet accessories" }
                };

                context.Shops.AddRange(shops);
                context.SaveChanges();

                // Create pet types
                var pets = new List<Pet>
                {
                    new Pet { AdminID = adminUser.ID, PetType = "Cat", Description = "A cute little cat" },
                    new Pet { AdminID = adminUser.ID, PetType = "Dog", Description = "A loyal dog" },
                    new Pet { AdminID = adminUser.ID, PetType = "Rabbit", Description = "A fluffy bunny" },
                    new Pet { AdminID = adminUser.ID, PetType = "Bird", Description = "A colorful bird" },
                    new Pet { AdminID = adminUser.ID, PetType = "Fish", Description = "A shiny fish" }
                };

                context.Pets.AddRange(pets);
                context.SaveChanges();

                // Create shop products
                var shopProducts = new List<ShopProduct>
                {
                    // Food products
                    new ShopProduct { ShopID = shops[1].ShopID, AdminID = adminUser.ID, Name = "Basic Cat Food", Type = "Food", Description = "Basic food for cats", ImageUrl = "/images/cat-food.png", Price = 10, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[1].ShopID, AdminID = adminUser.ID, Name = "Premium Dog Food", Type = "Food", Description = "Premium food for dogs", ImageUrl = "/images/dog-food.png", Price = 20, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[1].ShopID, AdminID = adminUser.ID, Name = "Rabbit Pellets", Type = "Food", Description = "Food for rabbits", ImageUrl = "/images/rabbit-food.png", Price = 15, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[1].ShopID, AdminID = adminUser.ID, Name = "Bird Seeds", Type = "Food", Description = "Seeds for birds", ImageUrl = "/images/bird-food.png", Price = 12, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[1].ShopID, AdminID = adminUser.ID, Name = "Fish Flakes", Type = "Food", Description = "Flakes for fish", ImageUrl = "/images/fish-food.png", Price = 8, CurrencyType = "Coin", Quality = 100 },
                    
                    // Toy products
                    new ShopProduct { ShopID = shops[2].ShopID, AdminID = adminUser.ID, Name = "Cat Toy Mouse", Type = "Toy", Description = "A toy mouse for cats", ImageUrl = "/images/cat-toy.png", Price = 15, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[2].ShopID, AdminID = adminUser.ID, Name = "Dog Bone", Type = "Toy", Description = "A bone for dogs", ImageUrl = "/images/dog-toy.png", Price = 25, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[2].ShopID, AdminID = adminUser.ID, Name = "Rabbit Ball", Type = "Toy", Description = "A ball for rabbits", ImageUrl = "/images/rabbit-toy.png", Price = 20, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[2].ShopID, AdminID = adminUser.ID, Name = "Bird Mirror", Type = "Toy", Description = "A mirror for birds", ImageUrl = "/images/bird-toy.png", Price = 18, CurrencyType = "Coin", Quality = 100 },
                    
                    // Accessory products
                    new ShopProduct { ShopID = shops[3].ShopID, AdminID = adminUser.ID, Name = "Cat Collar", Type = "Accessory", Description = "A collar for cats", ImageUrl = "/images/cat-collar.png", Price = 30, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[3].ShopID, AdminID = adminUser.ID, Name = "Dog Leash", Type = "Accessory", Description = "A leash for dogs", ImageUrl = "/images/dog-leash.png", Price = 40, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[3].ShopID, AdminID = adminUser.ID, Name = "Rabbit Hutch", Type = "Accessory", Description = "A hutch for rabbits", ImageUrl = "/images/rabbit-hutch.png", Price = 100, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[3].ShopID, AdminID = adminUser.ID, Name = "Bird Cage", Type = "Accessory", Description = "A cage for birds", ImageUrl = "/images/bird-cage.png", Price = 120, CurrencyType = "Coin", Quality = 100 },
                    new ShopProduct { ShopID = shops[3].ShopID, AdminID = adminUser.ID, Name = "Fish Tank", Type = "Accessory", Description = "A tank for fish", ImageUrl = "/images/fish-tank.png", Price = 150, CurrencyType = "Coin", Quality = 100 }
                };

                context.ShopProducts.AddRange(shopProducts);
                context.SaveChanges();

                // Create care activities
                var careActivities = new List<CareActivity>
                {
                    new CareActivity { ActivityType = "Feed", Description = "Feed your pet" },
                    new CareActivity { ActivityType = "Play", Description = "Play with your pet" },
                    new CareActivity { ActivityType = "Clean", Description = "Clean your pet or their living space" },
                    new CareActivity { ActivityType = "Sleep", Description = "Let your pet sleep" },
                    new CareActivity { ActivityType = "Exercise", Description = "Exercise your pet" },
                    new CareActivity { ActivityType = "Groom", Description = "Groom your pet" }
                };

                context.CareActivities.AddRange(careActivities);
                context.SaveChanges();

                // Create minigames
                var minigames = new List<Minigame>
                {
                    new Minigame { Name = "Pet Racing", Description = "Race your pet against others" },
                    new Minigame { Name = "Pet Puzzle", Description = "Solve puzzles with your pet" },
                    new Minigame { Name = "Pet Catch", Description = "Catch falling items with your pet" },
                    new Minigame { Name = "Pet Memory", Description = "Test your memory with your pet" }
                };

                context.Minigames.AddRange(minigames);
                context.SaveChanges();

                // Create achievements
                var achievements = new List<Achievement>
                {
                    new Achievement { AchievementName = "First Pet", Description = "Adopt your first pet" },
                    new Achievement { AchievementName = "Pet Master", Description = "Adopt 5 different pets" },
                    new Achievement { AchievementName = "Care Giver", Description = "Perform 50 care activities" },
                    new Achievement { AchievementName = "Shopper", Description = "Buy 10 different items" },
                    new Achievement { AchievementName = "Game Master", Description = "Score 1000 points in minigames" }
                };

                context.Achievements.AddRange(achievements);
                context.SaveChanges();
            }
        }
    }
}
