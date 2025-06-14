using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CareHistoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CareHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CareHistories
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CareHistoryDTO>>> GetAllCareHistories()
        {
            var careHistories = await _context.CareHistories
                .Include(ch => ch.Activity)
                .Include(ch => ch.Player)
                .Include(ch => ch.PlayerPet)
                    .ThenInclude(pp => pp.Pet)
                .Select(ch => new CareHistoryDTO
                {
                    CareHistoryID = ch.CareHistoryID,
                    PlayerPetID = ch.PlayerPetID,
                    PlayerID = ch.PlayerID,
                    ActivityID = ch.ActivityID,
                    PerformedAt = ch.PerformedAt,
                    PlayerName = ch.Player.UserName,
                    PetName = ch.PlayerPet.PetName,
                    Activity = new CareActivityDTO
                    {
                        ActivityID = ch.Activity.ActivityID,
                        ActivityType = ch.Activity.ActivityType,
                        Description = ch.Activity.Description
                    }
                })
                .ToListAsync();

            return careHistories;
        }

        // GET: api/CareHistories/user
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<CareHistoryDTO>>> GetUserCareHistories()
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            var careHistories = await _context.CareHistories
                .Where(ch => ch.PlayerID == userId)
                .Include(ch => ch.Activity)
                .Include(ch => ch.PlayerPet)
                .Select(ch => new CareHistoryDTO
                {
                    CareHistoryID = ch.CareHistoryID,
                    PlayerPetID = ch.PlayerPetID,
                    PlayerID = ch.PlayerID,
                    ActivityID = ch.ActivityID,
                    PerformedAt = ch.PerformedAt,
                    PlayerName = ch.Player.UserName,
                    PetName = ch.PlayerPet.PetName,
                    Activity = new CareActivityDTO
                    {
                        ActivityID = ch.Activity.ActivityID,
                        ActivityType = ch.Activity.ActivityType,
                        Description = ch.Activity.Description
                    }
                })
                .OrderByDescending(ch => ch.PerformedAt)
                .ToListAsync();

            return careHistories;
        }

        // GET: api/CareHistories/pet/5
        [HttpGet("pet/{playerPetId}")]
        public async Task<ActionResult<IEnumerable<CareHistoryDTO>>> GetPetCareHistories(int playerPetId)
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            // Verify the player pet belongs to the user
            var playerPet = await _context.PlayerPets
                .FirstOrDefaultAsync(pp => pp.PlayerPetID == playerPetId && pp.PlayerID == userId);

            if (playerPet == null)
            {
                return NotFound("Pet not found or doesn't belong to the current user");
            }

            var careHistories = await _context.CareHistories
                .Where(ch => ch.PlayerPetID == playerPetId)
                .Include(ch => ch.Activity)
                .Select(ch => new CareHistoryDTO
                {
                    CareHistoryID = ch.CareHistoryID,
                    PlayerPetID = ch.PlayerPetID,
                    PlayerID = ch.PlayerID,
                    ActivityID = ch.ActivityID,
                    PerformedAt = ch.PerformedAt,
                    PlayerName = ch.Player.UserName,
                    PetName = ch.PlayerPet.PetName,
                    Activity = new CareActivityDTO
                    {
                        ActivityID = ch.Activity.ActivityID,
                        ActivityType = ch.Activity.ActivityType,
                        Description = ch.Activity.Description
                    }
                })
                .OrderByDescending(ch => ch.PerformedAt)
                .ToListAsync();

            return careHistories;
        }

        // POST: api/CareHistories
        [HttpPost]
        public async Task<ActionResult<CareHistoryDTO>> CreateCareHistory(CreateCareHistoryDTO createCareHistoryDTO)
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            // Verify the player pet exists and belongs to the user
            var playerPet = await _context.PlayerPets
                .Include(pp => pp.Pet)
                .FirstOrDefaultAsync(pp => pp.PlayerPetID == createCareHistoryDTO.PlayerPetID && pp.PlayerID == userId);

            if (playerPet == null)
            {
                return NotFound("Pet not found or doesn't belong to the current user");
            }

            // Verify the activity exists
            var activity = await _context.CareActivities.FindAsync(createCareHistoryDTO.ActivityID);
            if (activity == null)
            {
                return NotFound("Care activity not found");
            }

            // Create the care history record
            var careHistory = new CareHistory
            {
                PlayerPetID = createCareHistoryDTO.PlayerPetID,
                PlayerID = userId,
                ActivityID = createCareHistoryDTO.ActivityID,
                PerformedAt = DateTime.Now
            };

            _context.CareHistories.Add(careHistory);
            
            // Update pet status based on activity type
            UpdatePetStatus(playerPet, activity.ActivityType);

            await _context.SaveChangesAsync();

            // Return the created care history
            var result = new CareHistoryDTO
            {
                CareHistoryID = careHistory.CareHistoryID,
                PlayerPetID = careHistory.PlayerPetID,
                PlayerID = careHistory.PlayerID,
                ActivityID = careHistory.ActivityID,
                PerformedAt = careHistory.PerformedAt,
                PlayerName = User.FindFirstValue(ClaimTypes.Name),
                PetName = playerPet.PetName,
                Activity = new CareActivityDTO
                {
                    ActivityID = activity.ActivityID,
                    ActivityType = activity.ActivityType,
                    Description = activity.Description
                }
            };

            return CreatedAtAction("GetPetCareHistories", new { playerPetId = playerPet.PlayerPetID }, result);
        }

        // DELETE: api/CareHistories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCareHistory(int id)
        {
            var careHistory = await _context.CareHistories.FindAsync(id);
            if (careHistory == null)
            {
                return NotFound();
            }

            _context.CareHistories.Remove(careHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void UpdatePetStatus(PlayerPet playerPet, string activityType)
        {
            // Update pet status based on activity type
            // This is a simple example - you might want to implement more complex logic
            switch (activityType.ToLower())
            {
                case "feed":
                    playerPet.Status = "Fed";
                    break;
                case "play":
                    playerPet.Status = "Happy";
                    break;
                case "bath":
                    playerPet.Status = "Clean";
                    break;
                case "sleep":
                    playerPet.Status = "Rested";
                    break;
                case "train":
                    playerPet.Status = "Trained";
                    playerPet.Level += 1; // Increment pet level on training
                    break;
                default:
                    playerPet.Status = "Normal";
                    break;
            }
            
            playerPet.LastStatusUpdate = DateTime.Now;
        }
    }
}
