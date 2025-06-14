using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AchievementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Achievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AchievementDTO>>> GetAchievements()
        {
            var achievements = await _context.Achievements
                .Select(a => new AchievementDTO
                {
                    AchievementID = a.AchievementID,
                    AchievementName = a.AchievementName,
                    Description = a.Description
                })
                .ToListAsync();

            return achievements;
        }

        // GET: api/Achievements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AchievementDTO>> GetAchievement(int id)
        {
            var achievement = await _context.Achievements
                .Where(a => a.AchievementID == id)
                .Select(a => new AchievementDTO
                {
                    AchievementID = a.AchievementID,
                    AchievementName = a.AchievementName,
                    Description = a.Description
                })
                .FirstOrDefaultAsync();

            if (achievement == null)
            {
                return NotFound();
            }

            return achievement;
        }

        // POST: api/Achievements
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AchievementDTO>> CreateAchievement(CreateAchievementDTO createAchievementDTO)
        {
            var achievement = new Achievement
            {
                AchievementName = createAchievementDTO.AchievementName,
                Description = createAchievementDTO.Description
            };

            _context.Achievements.Add(achievement);
            await _context.SaveChangesAsync();

            var dto = new AchievementDTO
            {
                AchievementID = achievement.AchievementID,
                AchievementName = achievement.AchievementName,
                Description = achievement.Description
            };

            return CreatedAtAction(nameof(GetAchievement), new { id = achievement.AchievementID }, dto);
        }

        // PUT: api/Achievements/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAchievement(int id, UpdateAchievementDTO updateAchievementDTO)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            if (updateAchievementDTO.AchievementName != null)
            {
                achievement.AchievementName = updateAchievementDTO.AchievementName;
            }
            
            if (updateAchievementDTO.Description != null)
            {
                achievement.Description = updateAchievementDTO.Description;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AchievementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Achievements/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            // Check if achievement is earned by any player
            var achievementInUse = await _context.PlayerAchievements.AnyAsync(pa => pa.AchievementID == id);
            if (achievementInUse)
            {
                return BadRequest("Cannot delete achievement because it has been earned by players");
            }

            _context.Achievements.Remove(achievement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AchievementExists(int id)
        {
            return _context.Achievements.Any(e => e.AchievementID == id);
        }
    }
}
