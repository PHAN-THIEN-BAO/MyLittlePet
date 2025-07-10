using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameStatsController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public GameStatsController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: GameStats - Get comprehensive game statistics
        [HttpGet]
        public ActionResult GetGameStats()
        {
            try
            {
                var totalUsers = _context.Users.Count();
                // Since UserStatus is removed, we'll just count all users as active
                var activeUsers = _context.Users.Count();
                var bannedUsers = 0; // No banned users since UserStatus is removed
                var onlineUsers = 0; // No online status tracking since UserStatus is removed
                
                var totalPets = _context.Pets.Count();
                var activePets = _context.Pets.Count(p => p.PetStatus == 1);
                var adoptedPets = _context.PlayerPets.Count();
                
                var totalShopProducts = _context.ShopProducts.Count();
                var availableProducts = _context.ShopProducts.Count(sp => sp.Status == 1);
                var inStockProducts = _context.ShopProducts.Count(sp => sp.Quantity > 0);
                
                var totalAchievements = _context.Achievements.Count();
                var earnedAchievements = _context.PlayerAchievements.Count();
                var collectedAchievements = _context.PlayerAchievements.Count(pa => pa.IsCollected == true);
                
                var totalMinigames = _context.Minigames.Count();
                var totalGameSessions = _context.GameRecords.Count();
                var averageScore = _context.GameRecords.Average(gr => (double?)gr.Score) ?? 0;
                
                var totalCareActivities = _context.CareActivities.Count();
                var totalCareSessionsPerformed = _context.CareHistories.Count();
                
                var totalInventoryItems = _context.PlayerInventories.Sum(pi => pi.Quantity ?? 0);
                
                var stats = new
                {
                    GameOverview = new
                    {
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        BannedUsers = bannedUsers,
                        OnlineUsers = onlineUsers
                    },
                    PetStatistics = new
                    {
                        TotalPetTypes = totalPets,
                        ActivePetTypes = activePets,
                        TotalAdoptedPets = adoptedPets,
                        AdoptionRate = totalPets > 0 ? Math.Round((double)adoptedPets / totalPets * 100, 2) : 0
                    },
                    ShopStatistics = new
                    {
                        TotalProducts = totalShopProducts,
                        AvailableProducts = availableProducts,
                        InStockProducts = inStockProducts,
                        OutOfStockProducts = totalShopProducts - inStockProducts
                    },
                    AchievementStatistics = new
                    {
                        TotalAchievements = totalAchievements,
                        EarnedAchievements = earnedAchievements,
                        CollectedAchievements = collectedAchievements,
                        UncollectedAchievements = earnedAchievements - collectedAchievements,
                        CollectionRate = earnedAchievements > 0 ? Math.Round((double)collectedAchievements / earnedAchievements * 100, 2) : 0
                    },
                    GameplayStatistics = new
                    {
                        TotalMinigames = totalMinigames,
                        TotalGameSessions = totalGameSessions,
                        AverageScore = Math.Round(averageScore, 2),
                        TotalCareActivityTypes = totalCareActivities,
                        TotalCareSessionsPerformed = totalCareSessionsPerformed
                    },
                    EconomyStatistics = new
                    {
                        TotalInventoryItems = totalInventoryItems,
                        TotalCoins = _context.Users.Sum(u => u.Coin ?? 0),
                        TotalDiamonds = _context.Users.Sum(u => u.Diamond ?? 0),
                        TotalGems = _context.Users.Sum(u => u.Gem ?? 0)
                    }
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: GameStats/TopPlayers - Get top players by various metrics
        [HttpGet("TopPlayers")]
        public ActionResult GetTopPlayers()
        {
            try
            {
                var topPlayersByLevel = _context.Users
                    .Where(u => u.Role == "Player") // Filter by Player role instead of UserStatus
                    .OrderByDescending(u => u.Level)
                    .ThenByDescending(u => u.Exp)
                    .Take(10)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Level,
                        Experience = u.Exp,
                        u.Coin,
                        u.Diamond,
                        u.Gem
                    })
                    .ToList();
                
                var topPlayersByWealth = _context.Users
                    .Where(u => u.Role == "Player") // Filter by Player role instead of UserStatus
                    .OrderByDescending(u => (u.Coin ?? 0) + (u.Diamond ?? 0) * 10 + (u.Gem ?? 0) * 5)
                    .Take(10)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Coin,
                        u.Diamond,
                        u.Gem,
                        TotalWealth = (u.Coin ?? 0) + (u.Diamond ?? 0) * 10 + (u.Gem ?? 0) * 5
                    })
                    .ToList();
                
                var topPlayersByPets = _context.Users
                    .Where(u => u.Role == "Player") // Filter by Player role instead of UserStatus
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        PetCount = _context.PlayerPets.Count(pp => pp.PlayerId == u.Id)
                    })
                    .OrderByDescending(u => u.PetCount)
                    .Take(10)
                    .ToList();
                
                var result = new
                {
                    TopPlayersByLevel = topPlayersByLevel,
                    TopPlayersByWealth = topPlayersByWealth,
                    TopPlayersByPetCount = topPlayersByPets
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: GameStats/DailyActivity - Get daily activity statistics
        [HttpGet("DailyActivity")]
        public ActionResult GetDailyActivity()
        {
            try
            {
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);
                
                var todayStats = new
                {
                    NewUsers = _context.Users.Count(u => u.JoinDate >= today),
                    PetsAdopted = _context.PlayerPets.Count(pp => pp.AdoptedAt >= today),
                    AchievementsEarned = _context.PlayerAchievements.Count(pa => pa.EarnedAt >= today),
                    AchievementsCollected = _context.PlayerAchievements.Count(pa => pa.EarnedAt >= today && pa.IsCollected == true),
                    GameSessionsPlayed = _context.GameRecords.Count(gr => gr.PlayedAt >= today),
                    CareActivitiesPerformed = _context.CareHistories.Count(ch => ch.PerformedAt >= today)
                };
                
                var yesterdayStats = new
                {
                    NewUsers = _context.Users.Count(u => u.JoinDate >= yesterday && u.JoinDate < today),
                    PetsAdopted = _context.PlayerPets.Count(pp => pp.AdoptedAt >= yesterday && pp.AdoptedAt < today),
                    AchievementsEarned = _context.PlayerAchievements.Count(pa => pa.EarnedAt >= yesterday && pa.EarnedAt < today),
                    AchievementsCollected = _context.PlayerAchievements.Count(pa => pa.EarnedAt >= yesterday && pa.EarnedAt < today && pa.IsCollected == true),
                    GameSessionsPlayed = _context.GameRecords.Count(gr => gr.PlayedAt >= yesterday && gr.PlayedAt < today),
                    CareActivitiesPerformed = _context.CareHistories.Count(ch => ch.PerformedAt >= yesterday && ch.PerformedAt < today)
                };
                
                var result = new
                {
                    Today = todayStats,
                    Yesterday = yesterdayStats,
                    Growth = new
                    {
                        NewUsersChange = todayStats.NewUsers - yesterdayStats.NewUsers,
                        PetsAdoptedChange = todayStats.PetsAdopted - yesterdayStats.PetsAdopted,
                        AchievementsEarnedChange = todayStats.AchievementsEarned - yesterdayStats.AchievementsEarned,
                        GameSessionsChange = todayStats.GameSessionsPlayed - yesterdayStats.GameSessionsPlayed
                    }
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
