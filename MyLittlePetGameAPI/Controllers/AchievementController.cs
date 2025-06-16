using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AchievementController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public AchievementController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: Achievement - Get all achievements
        [HttpGet]
        public ActionResult<IEnumerable<Achievement>> Get()
        {
            return Ok(_context.Achievements.ToList());
        }
        
        // GET: Achievement/{id} - Get achievement by ID
        [HttpGet("{id}")]
        public ActionResult<Achievement> GetById(int id)
        {
            var achievement = _context.Achievements.Find(id);
            
            if (achievement == null)
            {
                return NotFound();
            }
            
            return Ok(achievement);
        }
        
        // POST: Achievement - Create a new achievement
        [HttpPost]
        public ActionResult<Achievement> Create(string achievementName, string? description)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(achievementName))
            {
                return BadRequest("Achievement name is required");
            }
            
            var achievement = new Achievement
            {
                AchievementName = achievementName,
                Description = description
            };
            
            _context.Achievements.Add(achievement);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = achievement.AchievementId }, achievement);
        }
        
        // PUT: Achievement/{id} - Update an achievement
        [HttpPut("{id}")]
        public ActionResult<Achievement> Update(int id, string? achievementName, string? description)
        {
            var achievement = _context.Achievements.Find(id);
            
            if (achievement == null)
            {
                return NotFound();
            }
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(achievementName))
            {
                achievement.AchievementName = achievementName;
            }
            
            if (description != null) // Allow setting description to null
            {
                achievement.Description = description;
            }
            
            _context.Achievements.Update(achievement);
            _context.SaveChanges();
            
            return Ok(achievement);
        }
        
        // DELETE: Achievement/{id} - Delete an achievement
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var achievement = _context.Achievements.Find(id);
            
            if (achievement == null)
            {
                return NotFound();
            }
            
            // Check if the achievement is earned by any player
            var isEarned = _context.PlayerAchievements.Any(pa => pa.AchievementId == id);
            if (isEarned)
            {
                return BadRequest("Cannot delete achievement that has been earned by players");
            }
            
            _context.Achievements.Remove(achievement);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
