using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using System.Security.Claims;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetPets()
        {
            var pets = await _context.Pets
                .Include(p => p.Admin)
                .ToListAsync();

            var petDtos = pets.Select(p => new PetDTO
            {
                PetID = p.PetID,
                AdminID = p.AdminID,
                PetType = p.PetType,
                Description = p.Description,
                AdminName = p.Admin?.UserName
            }).ToList();

            return Ok(petDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetPet(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Admin)
                .FirstOrDefaultAsync(p => p.PetID == id);

            if (pet == null)
            {
                return NotFound();
            }

            var petDto = new PetDTO
            {
                PetID = pet.PetID,
                AdminID = pet.AdminID,
                PetType = pet.PetType,
                Description = pet.Description,
                AdminName = pet.Admin?.UserName
            };

            return Ok(petDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PetDTO>> CreatePet(CreatePetDTO createPetDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var pet = new Pet
            {
                AdminID = userId,
                PetType = createPetDto.PetType,
                Description = createPetDto.Description
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            var admin = await _context.Users.FindAsync(userId);

            var petDto = new PetDTO
            {
                PetID = pet.PetID,
                AdminID = pet.AdminID,
                PetType = pet.PetType,
                Description = pet.Description,
                AdminName = admin.UserName
            };

            return CreatedAtAction(nameof(GetPet), new { id = pet.PetID }, petDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePet(int id, UpdatePetDTO updatePetDto)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updatePetDto.PetType))
            {
                pet.PetType = updatePetDto.PetType;
            }

            if (updatePetDto.Description != null)
            {
                pet.Description = updatePetDto.Description;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetID == id);
        }
    }
}
