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
    public class MinigamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MinigamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Minigames
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MinigameDTO>>> GetMinigames()
        {
            var minigames = await _context.Minigames
                .Select(m => new MinigameDTO
                {
                    MinigameID = m.MinigameID,
                    Name = m.Name,
                    Description = m.Description
                })
                .ToListAsync();

            return minigames;
        }

        // GET: api/Minigames/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MinigameDTO>> GetMinigame(int id)
        {
            var minigame = await _context.Minigames
                .Where(m => m.MinigameID == id)
                .Select(m => new MinigameDTO
                {
                    MinigameID = m.MinigameID,
                    Name = m.Name,
                    Description = m.Description
                })
                .FirstOrDefaultAsync();

            if (minigame == null)
            {
                return NotFound();
            }

            return minigame;
        }

        // POST: api/Minigames
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MinigameDTO>> CreateMinigame(CreateMinigameDTO createMinigameDTO)
        {
            var minigame = new Minigame
            {
                Name = createMinigameDTO.Name,
                Description = createMinigameDTO.Description
            };

            _context.Minigames.Add(minigame);
            await _context.SaveChangesAsync();

            var dto = new MinigameDTO
            {
                MinigameID = minigame.MinigameID,
                Name = minigame.Name,
                Description = minigame.Description
            };

            return CreatedAtAction(nameof(GetMinigame), new { id = minigame.MinigameID }, dto);
        }

        // PUT: api/Minigames/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMinigame(int id, UpdateMinigameDTO updateMinigameDTO)
        {
            var minigame = await _context.Minigames.FindAsync(id);
            if (minigame == null)
            {
                return NotFound();
            }

            if (updateMinigameDTO.Name != null)
            {
                minigame.Name = updateMinigameDTO.Name;
            }
            
            if (updateMinigameDTO.Description != null)
            {
                minigame.Description = updateMinigameDTO.Description;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MinigameExists(id))
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

        // DELETE: api/Minigames/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMinigame(int id)
        {
            var minigame = await _context.Minigames.FindAsync(id);
            if (minigame == null)
            {
                return NotFound();
            }

            // Check if minigame has any game records
            var minigameInUse = await _context.GameRecords.AnyAsync(gr => gr.MinigameID == id);
            if (minigameInUse)
            {
                return BadRequest("Cannot delete minigame because it has game records");
            }

            _context.Minigames.Remove(minigame);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MinigameExists(int id)
        {
            return _context.Minigames.Any(e => e.MinigameID == id);
        }
    }
}
