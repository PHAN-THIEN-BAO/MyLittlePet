using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerAchievementController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public PlayerAchievementController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: PlayerAchievement - Get all player achievements
        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var achievements = _context.PlayerAchievements
                    .Include(pa => pa.Player)
                    .Include(pa => pa.Achievement)
                    .Select(pa => new 
                    {
                        UserId = pa.PlayerId,
                        AchievementId = pa.AchievementId,
                        IsCollected = pa.IsCollected
                    })
                    .ToList();
                    
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerAchievement/Player/{playerId} - Get achievements for a specific player
        [HttpGet("Player/{playerId}")]
        public ActionResult<IEnumerable<object>> GetByPlayerId(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var achievements = _context.PlayerAchievements
                    .Include(pa => pa.Achievement)
                    .Where(pa => pa.PlayerId == playerId)
                    .OrderByDescending(pa => pa.EarnedAt)
                    .Select(pa => new 
                    {
                        UserId = pa.PlayerId,
                        AchievementId = pa.AchievementId,
                        AchievementName = pa.Achievement.AchievementName,
                        Description = pa.Achievement.Description,
                        EarnedAt = pa.EarnedAt,
                        IsCollected = pa.IsCollected
                    })
                    .ToList();
                    
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerAchievement/Player/{playerId}/NotCollected - Get uncollected achievements for a player
        [HttpGet("Player/{playerId}/NotCollected")]
        public ActionResult<IEnumerable<object>> GetUncollectedByPlayerId(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var achievements = _context.PlayerAchievements
                    .Include(pa => pa.Achievement)
                    .Where(pa => pa.PlayerId == playerId && (pa.IsCollected == false || pa.IsCollected == null))
                    .OrderByDescending(pa => pa.EarnedAt)
                    .Select(pa => new 
                    {
                        AchievementId = pa.AchievementId,
                        AchievementName = pa.Achievement.AchievementName,
                        Description = pa.Achievement.Description,
                        EarnedAt = pa.EarnedAt,
                        IsCollected = pa.IsCollected
                    })
                    .ToList();
                    
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerAchievement/Achievement/{achievementId} - Get players who earned a specific achievement
        [HttpGet("Achievement/{achievementId}")]
        public ActionResult<IEnumerable<object>> GetByAchievementId(int achievementId)
        {
            try
            {
                var achievement = _context.Achievements.Find(achievementId);
                if (achievement == null)
                {
                    return NotFound("Achievement not found");
                }
                
                var players = _context.PlayerAchievements
                    .Include(pa => pa.Player)
                    .Where(pa => pa.AchievementId == achievementId)
                    .OrderByDescending(pa => pa.EarnedAt)
                    .Select(pa => new 
                    {
                        UserId = pa.PlayerId,
                        PlayerName = pa.Player.UserName,
                        EarnedAt = pa.EarnedAt,
                        IsCollected = pa.IsCollected
                    })
                    .ToList();
                    
                return Ok(players);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: PlayerAchievement - Award an achievement to a player
        [HttpPost]
        public ActionResult Create([FromQuery] int playerId, [FromQuery] int achievementId, [FromQuery] bool? isCollected = false)
        {
            try
            {
                // Validate player exists
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return BadRequest("Player not found");
                }
                
                // Validate achievement exists
                var achievement = _context.Achievements.Find(achievementId);
                if (achievement == null)
                {
                    return BadRequest("Achievement not found");
                }
                
                // Check if player already has this achievement
                var existingAchievement = _context.PlayerAchievements
                    .FirstOrDefault(pa => pa.PlayerId == playerId && pa.AchievementId == achievementId);
                    
                if (existingAchievement != null)
                {
                    return BadRequest("Player already has this achievement");
                }
                
                var playerAchievement = new PlayerAchievement
                {
                    PlayerId = playerId,
                    AchievementId = achievementId,
                    EarnedAt = DateTime.Now,
                    IsCollected = isCollected ?? false // Default to false if not provided
                };
                
                _context.PlayerAchievements.Add(playerAchievement);
                _context.SaveChanges();
                
                return Ok(new 
                {
                    message = "Achievement awarded successfully",
                    playerAchievement = new 
                    {
                        PlayerId = playerAchievement.PlayerId,
                        AchievementId = playerAchievement.AchievementId,
                        EarnedAt = playerAchievement.EarnedAt,
                        IsCollected = playerAchievement.IsCollected
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // DELETE: PlayerAchievement - Remove an achievement from a player
        [HttpDelete]
        public ActionResult Delete(int playerId, int achievementId)
        {
            try
            {
                var playerAchievement = _context.PlayerAchievements
                    .FirstOrDefault(pa => pa.PlayerId == playerId && pa.AchievementId == achievementId);
                    
                if (playerAchievement == null)
                {
                    return NotFound("Player does not have this achievement");
                }
                
                _context.PlayerAchievements.Remove(playerAchievement);
                _context.SaveChanges();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // PUT: PlayerAchievement/Collect - Mark an achievement as collected
        [HttpPut("Collect")]
        public ActionResult<PlayerAchievement> MarkAsCollected(int playerId, int achievementId, string isCollected)
        {
            try
            {
                var playerAchievement = _context.PlayerAchievements
                    .FirstOrDefault(pa => pa.PlayerId == playerId && pa.AchievementId == achievementId);
                    
                if (playerAchievement == null)
                {
                    return NotFound("Player does not have this achievement");
                }
                
                // Set IsCollected based on input string
                playerAchievement.IsCollected = string.Equals(isCollected, "true", StringComparison.OrdinalIgnoreCase);
                
                _context.PlayerAchievements.Update(playerAchievement);
                _context.SaveChanges();
                
                return Ok(playerAchievement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: PlayerAchievement/{playerId}/{achievementId}/Collect - Mark achievement as collected
        [HttpPut("{playerId}/{achievementId}/Collect")]
        public IActionResult CollectAchievement(int playerId, int achievementId)
        {
            try
            {
                var playerAchievement = _context.PlayerAchievements
                    .FirstOrDefault(pa => pa.PlayerId == playerId && pa.AchievementId == achievementId);
                
                if (playerAchievement == null)
                {
                    return NotFound("Achievement not found for this player");
                }
                
                if (playerAchievement.IsCollected == true)
                {
                    return BadRequest("Achievement already collected");
                }
                
                playerAchievement.IsCollected = true;
                _context.SaveChanges();
                
                return Ok(new { message = "Achievement collected successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerAchievement/Player/{playerId}/Collected - Get collected achievements
        [HttpGet("Player/{playerId}/Collected")]
        public ActionResult<IEnumerable<object>> GetCollectedAchievements(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var collectedAchievements = _context.PlayerAchievements
                    .Include(pa => pa.Achievement)
                    .Where(pa => pa.PlayerId == playerId && pa.IsCollected == true)
                    .OrderByDescending(pa => pa.EarnedAt)
                    .Select(pa => new 
                    {
                        AchievementId = pa.AchievementId,
                        AchievementName = pa.Achievement.AchievementName,
                        Description = pa.Achievement.Description,
                        EarnedAt = pa.EarnedAt,
                        IsCollected = pa.IsCollected
                    })
                    .ToList();
                
                return Ok(collectedAchievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerAchievement/Player/{playerId}/Stats - Get achievement statistics
        [HttpGet("Player/{playerId}/Stats")]
        public ActionResult GetAchievementStats(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var totalAchievements = _context.PlayerAchievements.Count(pa => pa.PlayerId == playerId);
                var collectedAchievements = _context.PlayerAchievements.Count(pa => pa.PlayerId == playerId && pa.IsCollected == true);
                var uncollectedAchievements = totalAchievements - collectedAchievements;
                var collectionRate = totalAchievements > 0 ? Math.Round((double)collectedAchievements / totalAchievements * 100, 2) : 0;
                
                var stats = new
                {
                    PlayerId = playerId,
                    PlayerName = player.UserName,
                    TotalAchievements = totalAchievements,
                    CollectedAchievements = collectedAchievements,
                    UncollectedAchievements = uncollectedAchievements,
                    CollectionRate = collectionRate,
                    LastAchievementEarned = _context.PlayerAchievements
                        .Where(pa => pa.PlayerId == playerId)
                        .OrderByDescending(pa => pa.EarnedAt)
                        .Select(pa => pa.EarnedAt)
                        .FirstOrDefault()
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // Request model for creating player achievements
    public class CreatePlayerAchievementRequest
    {
        public int PlayerId { get; set; }
        public int AchievementId { get; set; }
        public bool? IsCollected { get; set; }
    }
}
