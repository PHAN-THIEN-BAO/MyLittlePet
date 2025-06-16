using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MinigameController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public MinigameController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: Minigame - Get all minigames
        [HttpGet]
        public ActionResult<IEnumerable<Minigame>> Get()
        {
            return Ok(_context.Minigames.ToList());
        }
        
        // GET: Minigame/{id} - Get minigame by ID
        [HttpGet("{id}")]
        public ActionResult<Minigame> GetById(int id)
        {
            var minigame = _context.Minigames.Find(id);
            
            if (minigame == null)
            {
                return NotFound();
            }
            
            return Ok(minigame);
        }
        
        // POST: Minigame - Create a new minigame
        [HttpPost]
        public ActionResult<Minigame> Create(string name, string? description)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Minigame name is required");
            }
            
            var minigame = new Minigame
            {
                Name = name,
                Description = description
            };
            
            _context.Minigames.Add(minigame);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = minigame.MinigameId }, minigame);
        }
        
        // PUT: Minigame/{id} - Update a minigame
        [HttpPut("{id}")]
        public ActionResult<Minigame> Update(int id, string? name, string? description)
        {
            var minigame = _context.Minigames.Find(id);
            
            if (minigame == null)
            {
                return NotFound();
            }
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(name))
            {
                minigame.Name = name;
            }
            
            if (description != null) // Allow setting description to null
            {
                minigame.Description = description;
            }
            
            _context.Minigames.Update(minigame);
            _context.SaveChanges();
            
            return Ok(minigame);
        }
        
        // DELETE: Minigame/{id} - Delete a minigame
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var minigame = _context.Minigames.Find(id);
            
            if (minigame == null)
            {
                return NotFound();
            }
            
            // Check if minigame has game records
            var hasRecords = _context.GameRecords.Any(gr => gr.MinigameId == id);
            if (hasRecords)
            {
                return BadRequest("Cannot delete minigame with existing game records");
            }
            
            _context.Minigames.Remove(minigame);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
