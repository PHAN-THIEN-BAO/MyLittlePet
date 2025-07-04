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
        
        // GET: PlayerAchievement/Player/{playerId} - Get achievements for a specific player
        [HttpGet("Player/{playerId}")]
        public ActionResult<IEnumerable<object>> GetByPlayerId(int playerId)
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
                    IsCollected = pa.IsCollected
                })
                .ToList();
                
            return Ok(achievements);
        }
        
        // GET: PlayerAchievement/Player/{playerId}/NotCollected - Get uncollected achievements for a player
        [HttpGet("Player/{playerId}/NotCollected")]
        public ActionResult<IEnumerable<PlayerAchievement>> GetUncollectedByPlayerId(int playerId)
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
                .ToList();
                
            return Ok(achievements);
        }
        
        // GET: PlayerAchievement/Achievement/{achievementId} - Get players who earned a specific achievement
        [HttpGet("Achievement/{achievementId}")]
        public ActionResult<IEnumerable<PlayerAchievement>> GetByAchievementId(int achievementId)
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
                .ToList();
                
            return Ok(players);
        }
        
        // POST: PlayerAchievement - Award an achievement to a player
        [HttpPost]
        public ActionResult<PlayerAchievement> Create(int playerId, int achievementId, bool? isCollected)
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
            
            return CreatedAtAction("GetUncollectedByPlayerId", new { playerId = playerId }, playerAchievement);
        }
        
        // DELETE: PlayerAchievement - Remove an achievement from a player
        [HttpDelete]
        public ActionResult Delete(int playerId, int achievementId)
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
        
        // PUT: PlayerAchievement/Collect - Mark an achievement as collected
        [HttpPut("Collect")]
        public ActionResult<PlayerAchievement> MarkAsCollected(int playerId, int achievementId, string isCollected)
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
        
        // GET: PlayerAchievement/Player/{playerId}/Uncollected - Get uncollected achievements
        [HttpGet("Player/{playerId}/Uncollected")]
        public ActionResult<IEnumerable<object>> GetUncollectedAchievements(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var uncollectedAchievements = _context.PlayerAchievements
                    .Include(pa => pa.Achievement)
                    .Where(pa => pa.PlayerId == playerId && pa.IsCollected == false)
                    .OrderBy(pa => pa.EarnedAt)
                    .Select(pa => new 
                    {
                        AchievementId = pa.AchievementId,
                        AchievementName = pa.Achievement.AchievementName,
                        Description = pa.Achievement.Description,
                        EarnedAt = pa.EarnedAt,
                        IsCollected = pa.IsCollected
                    })
                    .ToList();
                
                return Ok(uncollectedAchievements);
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
}
