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
    public class PlayerPetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerPetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerPetDTO>>> GetPlayerPets()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<PlayerPet> query = _context.PlayerPets
                .Include(pp => pp.Player)
                .Include(pp => pp.Pet)
                .ThenInclude(p => p.Admin);

            // If not admin, only show the user's pets
            if (userRole != "Admin")
            {
                query = query.Where(pp => pp.PlayerID == userId);
            }

            var playerPets = await query.ToListAsync();

            var playerPetDtos = playerPets.Select(pp => new PlayerPetDTO
            {
                PlayerPetID = pp.PlayerPetID,
                PlayerID = pp.PlayerID,
                PetID = pp.PetID,
                PetName = pp.PetName,
                AdoptedAt = pp.AdoptedAt,
                Level = pp.Level,
                Status = pp.Status,
                LastStatusUpdate = pp.LastStatusUpdate,
                PlayerName = pp.Player.UserName,
                Pet = new PetDTO
                {
                    PetID = pp.Pet.PetID,
                    AdminID = pp.Pet.AdminID,
                    PetType = pp.Pet.PetType,
                    Description = pp.Pet.Description,
                    AdminName = pp.Pet.Admin?.UserName
                }
            }).ToList();

            return Ok(playerPetDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerPetDTO>> GetPlayerPet(int id)
        {
            var playerPet = await _context.PlayerPets
                .Include(pp => pp.Player)
                .Include(pp => pp.Pet)
                .ThenInclude(p => p.Admin)
                .FirstOrDefaultAsync(pp => pp.PlayerPetID == id);

            if (playerPet == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to view their own pets unless they're an admin
            if (playerPet.PlayerID != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var playerPetDto = new PlayerPetDTO
            {
                PlayerPetID = playerPet.PlayerPetID,
                PlayerID = playerPet.PlayerID,
                PetID = playerPet.PetID,
                PetName = playerPet.PetName,
                AdoptedAt = playerPet.AdoptedAt,
                Level = playerPet.Level,
                Status = playerPet.Status,
                LastStatusUpdate = playerPet.LastStatusUpdate,
                PlayerName = playerPet.Player.UserName,
                Pet = new PetDTO
                {
                    PetID = playerPet.Pet.PetID,
                    AdminID = playerPet.Pet.AdminID,
                    PetType = playerPet.Pet.PetType,
                    Description = playerPet.Pet.Description,
                    AdminName = playerPet.Pet.Admin?.UserName
                }
            };

            return Ok(playerPetDto);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerPetDTO>> AdoptPet(CreatePlayerPetDTO createPlayerPetDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Check if pet exists
            var pet = await _context.Pets.FindAsync(createPlayerPetDto.PetID);
            if (pet == null)
            {
                return NotFound("Pet not found");
            }

            // Check if the player already has a pet with the same name
            var petNameExists = await _context.PlayerPets
                .AnyAsync(pp => pp.PlayerID == userId && pp.PetName == createPlayerPetDto.PetName);

            if (petNameExists)
            {
                return BadRequest("You already have a pet with this name");
            }

            var playerPet = new PlayerPet
            {
                PlayerID = userId,
                PetID = createPlayerPetDto.PetID,
                PetName = createPlayerPetDto.PetName,
                AdoptedAt = DateTime.Now,
                Level = 0,
                Status = "Happy",
                LastStatusUpdate = DateTime.Now
            };

            _context.PlayerPets.Add(playerPet);
            await _context.SaveChangesAsync();

            var player = await _context.Users.FindAsync(userId);
            var petWithAdmin = await _context.Pets
                .Include(p => p.Admin)
                .FirstOrDefaultAsync(p => p.PetID == createPlayerPetDto.PetID);

            var playerPetDto = new PlayerPetDTO
            {
                PlayerPetID = playerPet.PlayerPetID,
                PlayerID = playerPet.PlayerID,
                PetID = playerPet.PetID,
                PetName = playerPet.PetName,
                AdoptedAt = playerPet.AdoptedAt,
                Level = playerPet.Level,
                Status = playerPet.Status,
                LastStatusUpdate = playerPet.LastStatusUpdate,
                PlayerName = player.UserName,
                Pet = new PetDTO
                {
                    PetID = petWithAdmin.PetID,
                    AdminID = petWithAdmin.AdminID,
                    PetType = petWithAdmin.PetType,
                    Description = petWithAdmin.Description,
                    AdminName = petWithAdmin.Admin?.UserName
                }
            };

            return CreatedAtAction(nameof(GetPlayerPet), new { id = playerPet.PlayerPetID }, playerPetDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayerPet(int id, UpdatePlayerPetDTO updatePlayerPetDto)
        {
            var playerPet = await _context.PlayerPets.FindAsync(id);

            if (playerPet == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to update their own pets unless they're an admin
            if (playerPet.PlayerID != userId && userRole != "Admin")
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(updatePlayerPetDto.PetName))
            {
                // Check if the player already has a pet with the same name
                var petNameExists = await _context.PlayerPets
                    .AnyAsync(pp => pp.PlayerID == playerPet.PlayerID && 
                                    pp.PetName == updatePlayerPetDto.PetName && 
                                    pp.PlayerPetID != id);

                if (petNameExists)
                {
                    return BadRequest("You already have a pet with this name");
                }

                playerPet.PetName = updatePlayerPetDto.PetName;
            }

            if (!string.IsNullOrEmpty(updatePlayerPetDto.Status))
            {
                playerPet.Status = updatePlayerPetDto.Status;
                playerPet.LastStatusUpdate = DateTime.Now;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerPetExists(id))
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

        [HttpPut("{id}/level")]
        public async Task<IActionResult> UpdatePlayerPetLevel(int id, [FromBody] Dictionary<string, int> levelUpdate)
        {
            var playerPet = await _context.PlayerPets.FindAsync(id);

            if (playerPet == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to update their own pets unless they're an admin
            if (playerPet.PlayerID != userId && userRole != "Admin")
            {
                return Forbid();
            }

            if (levelUpdate.ContainsKey("level"))
            {
                playerPet.Level = levelUpdate["level"];
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerPetExists(id))
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
        public async Task<IActionResult> DeletePlayerPet(int id)
        {
            var playerPet = await _context.PlayerPets.FindAsync(id);
            if (playerPet == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to delete their own pets unless they're an admin
            if (playerPet.PlayerID != userId && userRole != "Admin")
            {
                return Forbid();
            }

            _context.PlayerPets.Remove(playerPet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerPetExists(int id)
        {
            return _context.PlayerPets.Any(e => e.PlayerPetID == id);
        }
    }
}
