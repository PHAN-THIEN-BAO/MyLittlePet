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
        public ActionResult<IEnumerable<PlayerAchievement>> Get()
        {
            return Ok(_context.PlayerAchievements
                .Include(pa => pa.Player)
                .Include(pa => pa.Achievement)
                .ToList());
        }
        
        // GET: PlayerAchievement/Player/{playerId} - Get achievements for a specific player
        [HttpGet("Player/{playerId}")]
        public ActionResult<IEnumerable<PlayerAchievement>> GetByPlayerId(int playerId)
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
        public ActionResult<PlayerAchievement> Create(int playerId, int achievementId)
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
                EarnedAt = DateTime.Now
            };
            
            _context.PlayerAchievements.Add(playerAchievement);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetByPlayerId), new { playerId = playerId }, playerAchievement);
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
    }
}
