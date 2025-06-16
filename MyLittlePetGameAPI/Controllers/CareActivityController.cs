using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CareActivityController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public CareActivityController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: CareActivity - Get all care activities
        [HttpGet]
        public ActionResult<IEnumerable<CareActivity>> Get()
        {
            return Ok(_context.CareActivities.ToList());
        }
        
        // GET: CareActivity/{id} - Get care activity by ID
        [HttpGet("{id}")]
        public ActionResult<CareActivity> GetById(int id)
        {
            var activity = _context.CareActivities.Find(id);
            
            if (activity == null)
            {
                return NotFound();
            }
            
            return Ok(activity);
        }
        
        // GET: CareActivity/Type/{type} - Get activities by type
        [HttpGet("Type/{type}")]
        public ActionResult<IEnumerable<CareActivity>> GetByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest("Activity type is required");
            }
            
            var activities = _context.CareActivities
                .Where(a => a.ActivityType.Contains(type))
                .ToList();
                
            return Ok(activities);
        }
        
        // POST: CareActivity - Create a new care activity
        [HttpPost]
        public ActionResult<CareActivity> Create(string activityType, string? description)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(activityType))
            {
                return BadRequest("Activity type is required");
            }
            
            var activity = new CareActivity
            {
                ActivityType = activityType,
                Description = description
            };
            
            _context.CareActivities.Add(activity);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = activity.ActivityId }, activity);
        }
        
        // PUT: CareActivity/{id} - Update a care activity
        [HttpPut("{id}")]
        public ActionResult<CareActivity> Update(int id, string? activityType, string? description)
        {
            var activity = _context.CareActivities.Find(id);
            
            if (activity == null)
            {
                return NotFound();
            }
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(activityType))
            {
                activity.ActivityType = activityType;
            }
            
            if (description != null) // Allow setting description to null
            {
                activity.Description = description;
            }
            
            _context.CareActivities.Update(activity);
            _context.SaveChanges();
            
            return Ok(activity);
        }
        
        // DELETE: CareActivity/{id} - Delete a care activity
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var activity = _context.CareActivities.Find(id);
            
            if (activity == null)
            {
                return NotFound();
            }
            
            // Check if the activity is used in care history
            var isUsed = _context.CareHistories.Any(h => h.ActivityId == id);
            if (isUsed)
            {
                return BadRequest("Cannot delete activity that is used in care history records");
            }
            
            _context.CareActivities.Remove(activity);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
