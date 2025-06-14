using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlayerAchievementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerAchievementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerAchievements
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PlayerAchievementDTO>>> GetAllPlayerAchievements()
        {
            var playerAchievements = await _context.PlayerAchievements
                .Include(pa => pa.Player)
                .Include(pa => pa.Achievement)
                .Select(pa => new PlayerAchievementDTO
                {
                    PlayerID = pa.PlayerID,
                    AchievementID = pa.AchievementID,
                    EarnedAt = pa.EarnedAt,
                    PlayerName = pa.Player.UserName,
                    Achievement = new AchievementDTO
                    {
                        AchievementID = pa.Achievement.AchievementID,
                        AchievementName = pa.Achievement.AchievementName,
                        Description = pa.Achievement.Description
                    }
                })
                .ToListAsync();

            return playerAchievements;
        }

        // GET: api/PlayerAchievements/user
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PlayerAchievementDTO>>> GetUserAchievements()
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            var playerAchievements = await _context.PlayerAchievements
                .Where(pa => pa.PlayerID == userId)
                .Include(pa => pa.Achievement)
                .Select(pa => new PlayerAchievementDTO
                {
                    PlayerID = pa.PlayerID,
                    AchievementID = pa.AchievementID,
                    EarnedAt = pa.EarnedAt,
                    PlayerName = User.FindFirstValue(ClaimTypes.Name),
                    Achievement = new AchievementDTO
                    {
                        AchievementID = pa.Achievement.AchievementID,
                        AchievementName = pa.Achievement.AchievementName,
                        Description = pa.Achievement.Description
                    }
                })
                .OrderByDescending(pa => pa.EarnedAt)
                .ToListAsync();

            return playerAchievements;
        }

        // POST: api/PlayerAchievements
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlayerAchievementDTO>> AwardAchievement(CreatePlayerAchievementDTO createPlayerAchievementDTO)
        {
            // Check if player exists
            var player = await _context.Users.FindAsync(createPlayerAchievementDTO.PlayerID);
            if (player == null)
            {
                return NotFound("Player not found");
            }

            // Check if achievement exists
            var achievement = await _context.Achievements.FindAsync(createPlayerAchievementDTO.AchievementID);
            if (achievement == null)
            {
                return NotFound("Achievement not found");
            }

            // Check if player already has this achievement
            var existingAchievement = await _context.PlayerAchievements
                .FirstOrDefaultAsync(pa => pa.PlayerID == createPlayerAchievementDTO.PlayerID && 
                                          pa.AchievementID == createPlayerAchievementDTO.AchievementID);
            
            if (existingAchievement != null)
            {
                return BadRequest("Player already has this achievement");
            }

            // Create new player achievement
            var playerAchievement = new PlayerAchievement
            {
                PlayerID = createPlayerAchievementDTO.PlayerID,
                AchievementID = createPlayerAchievementDTO.AchievementID,
                EarnedAt = DateTime.Now
            };

            _context.PlayerAchievements.Add(playerAchievement);
            await _context.SaveChangesAsync();

            // Return the created achievement
            var result = new PlayerAchievementDTO
            {
                PlayerID = playerAchievement.PlayerID,
                AchievementID = playerAchievement.AchievementID,
                EarnedAt = playerAchievement.EarnedAt,
                PlayerName = player.UserName,
                Achievement = new AchievementDTO
                {
                    AchievementID = achievement.AchievementID,
                    AchievementName = achievement.AchievementName,
                    Description = achievement.Description
                }
            };

            return CreatedAtAction("GetUserAchievements", result);
        }

        // DELETE: api/PlayerAchievements/{playerId}/{achievementId}
        [HttpDelete("{playerId}/{achievementId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlayerAchievement(int playerId, int achievementId)
        {
            var playerAchievement = await _context.PlayerAchievements
                .FirstOrDefaultAsync(pa => pa.PlayerID == playerId && pa.AchievementID == achievementId);
                
            if (playerAchievement == null)
            {
                return NotFound();
            }

            _context.PlayerAchievements.Remove(playerAchievement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/PlayerAchievements/check
        [HttpPost("check")]
        public async Task<IActionResult> CheckAndAwardAchievements()
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            // Get user details
            var user = await _context.Users
                .Include(u => u.PlayerPets)
                .Include(u => u.CareHistories)
                .Include(u => u.GameRecords)
                .Include(u => u.PlayerAchievements)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var awardedAchievements = new List<PlayerAchievementDTO>();

            // Check for pet adoption achievement
            if (user.PlayerPets.Count >= 1 && 
                !user.PlayerAchievements.Any(pa => pa.AchievementID == 1)) // Assuming ID 1 is for first pet adoption
            {
                var achievement = await _context.Achievements.FindAsync(1);
                if (achievement != null)
                {
                    var playerAchievement = new PlayerAchievement
                    {
                        PlayerID = userId,
                        AchievementID = 1,
                        EarnedAt = DateTime.Now
                    };

                    _context.PlayerAchievements.Add(playerAchievement);
                    await _context.SaveChangesAsync();

                    awardedAchievements.Add(new PlayerAchievementDTO
                    {
                        PlayerID = userId,
                        AchievementID = 1,
                        EarnedAt = playerAchievement.EarnedAt,
                        PlayerName = user.UserName,
                        Achievement = new AchievementDTO
                        {
                            AchievementID = achievement.AchievementID,
                            AchievementName = achievement.AchievementName,
                            Description = achievement.Description
                        }
                    });
                }
            }

            // Check for multiple pets achievement
            if (user.PlayerPets.Count >= 3 && 
                !user.PlayerAchievements.Any(pa => pa.AchievementID == 2)) // Assuming ID 2 is for having 3 pets
            {
                var achievement = await _context.Achievements.FindAsync(2);
                if (achievement != null)
                {
                    var playerAchievement = new PlayerAchievement
                    {
                        PlayerID = userId,
                        AchievementID = 2,
                        EarnedAt = DateTime.Now
                    };

                    _context.PlayerAchievements.Add(playerAchievement);
                    await _context.SaveChangesAsync();

                    awardedAchievements.Add(new PlayerAchievementDTO
                    {
                        PlayerID = userId,
                        AchievementID = 2,
                        EarnedAt = playerAchievement.EarnedAt,
                        PlayerName = user.UserName,
                        Achievement = new AchievementDTO
                        {
                            AchievementID = achievement.AchievementID,
                            AchievementName = achievement.AchievementName,
                            Description = achievement.Description
                        }
                    });
                }
            }

            // Check for care activities achievement
            var careActivitiesCount = user.CareHistories.Count;
            if (careActivitiesCount >= 10 && 
                !user.PlayerAchievements.Any(pa => pa.AchievementID == 3)) // Assuming ID 3 is for 10 care activities
            {
                var achievement = await _context.Achievements.FindAsync(3);
                if (achievement != null)
                {
                    var playerAchievement = new PlayerAchievement
                    {
                        PlayerID = userId,
                        AchievementID = 3,
                        EarnedAt = DateTime.Now
                    };

                    _context.PlayerAchievements.Add(playerAchievement);
                    await _context.SaveChangesAsync();

                    awardedAchievements.Add(new PlayerAchievementDTO
                    {
                        PlayerID = userId,
                        AchievementID = 3,
                        EarnedAt = playerAchievement.EarnedAt,
                        PlayerName = user.UserName,
                        Achievement = new AchievementDTO
                        {
                            AchievementID = achievement.AchievementID,
                            AchievementName = achievement.AchievementName,
                            Description = achievement.Description
                        }
                    });
                }
            }

            return Ok(new { newAchievements = awardedAchievements });
        }
    }
}
