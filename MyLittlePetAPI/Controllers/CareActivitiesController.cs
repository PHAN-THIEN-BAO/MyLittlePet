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
    public class CareActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CareActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CareActivities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CareActivityDTO>>> GetCareActivities()
        {
            var activities = await _context.CareActivities
                .Select(a => new CareActivityDTO
                {
                    ActivityID = a.ActivityID,
                    ActivityType = a.ActivityType,
                    Description = a.Description
                })
                .ToListAsync();

            return activities;
        }

        // GET: api/CareActivities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CareActivityDTO>> GetCareActivity(int id)
        {
            var careActivity = await _context.CareActivities
                .Where(a => a.ActivityID == id)
                .Select(a => new CareActivityDTO
                {
                    ActivityID = a.ActivityID,
                    ActivityType = a.ActivityType,
                    Description = a.Description
                })
                .FirstOrDefaultAsync();

            if (careActivity == null)
            {
                return NotFound();
            }

            return careActivity;
        }

        // POST: api/CareActivities
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CareActivityDTO>> CreateCareActivity(CreateCareActivityDTO createCareActivityDTO)
        {
            var careActivity = new CareActivity
            {
                ActivityType = createCareActivityDTO.ActivityType,
                Description = createCareActivityDTO.Description
            };

            _context.CareActivities.Add(careActivity);
            await _context.SaveChangesAsync();

            var dto = new CareActivityDTO
            {
                ActivityID = careActivity.ActivityID,
                ActivityType = careActivity.ActivityType,
                Description = careActivity.Description
            };

            return CreatedAtAction(nameof(GetCareActivity), new { id = careActivity.ActivityID }, dto);
        }

        // PUT: api/CareActivities/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCareActivity(int id, UpdateCareActivityDTO updateCareActivityDTO)
        {
            var careActivity = await _context.CareActivities.FindAsync(id);
            if (careActivity == null)
            {
                return NotFound();
            }

            if (updateCareActivityDTO.ActivityType != null)
            {
                careActivity.ActivityType = updateCareActivityDTO.ActivityType;
            }
            
            if (updateCareActivityDTO.Description != null)
            {
                careActivity.Description = updateCareActivityDTO.Description;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CareActivityExists(id))
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

        // DELETE: api/CareActivities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCareActivity(int id)
        {
            var careActivity = await _context.CareActivities.FindAsync(id);
            if (careActivity == null)
            {
                return NotFound();
            }

            // Check if activity is used in care history
            var activityInUse = await _context.CareHistories.AnyAsync(ch => ch.ActivityID == id);
            if (activityInUse)
            {
                return BadRequest("Cannot delete activity because it is used in care history records");
            }

            _context.CareActivities.Remove(careActivity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CareActivityExists(int id)
        {
            return _context.CareActivities.Any(e => e.ActivityID == id);
        }
    }
}
