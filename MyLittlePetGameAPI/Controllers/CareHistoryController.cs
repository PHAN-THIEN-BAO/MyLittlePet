using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CareHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public CareHistoryController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: CareHistory - Get all care history records
        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var histories = _context.CareHistories
                    .Include(h => h.PlayerPet)
                    .Include(h => h.Activity)
                    .Include(h => h.Player)
                    .Select(h => new
                    {
                        CareHistoryId = h.CareHistoryId,
                        PlayerPetId = h.PlayerPetId,
                        PlayerId = h.PlayerId,
                        ActivityId = h.ActivityId,
                        PerformedAt = h.PerformedAt,
                        PlayerPetInfo = new
                        {
                            PlayerPetId = h.PlayerPet.PlayerPetId,
                            PetCustomName = h.PlayerPet.PetCustomName,
                            PetId = h.PlayerPet.PetId
                        },
                        ActivityInfo = new
                        {
                            ActivityId = h.Activity.ActivityId,
                            ActivityType = h.Activity.ActivityType
                        },
                        PlayerInfo = new
                        {
                            Id = h.Player.Id,
                            UserName = h.Player.UserName
                        }
                    })
                    .ToList();
                
                return Ok(histories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: CareHistory/{id} - Get care history by ID
        [HttpGet("{id}")]
        public ActionResult<object> GetById(int id)
        {
            try
            {
                var history = _context.CareHistories
                    .Include(h => h.PlayerPet)
                    .Include(h => h.Activity)
                    .Include(h => h.Player)
                    .FirstOrDefault(h => h.CareHistoryId == id);
                
                if (history == null)
                {
                    return NotFound();
                }
                
                var result = new
                {
                    CareHistoryId = history.CareHistoryId,
                    PlayerPetId = history.PlayerPetId,
                    PlayerId = history.PlayerId,
                    ActivityId = history.ActivityId,
                    PerformedAt = history.PerformedAt,
                    PlayerPetInfo = new
                    {
                        PlayerPetId = history.PlayerPet.PlayerPetId,
                        PetCustomName = history.PlayerPet.PetCustomName,
                        PetId = history.PlayerPet.PetId
                    },
                    ActivityInfo = new
                    {
                        ActivityId = history.Activity.ActivityId,
                        ActivityType = history.Activity.ActivityType
                    },
                    PlayerInfo = new
                    {
                        Id = history.Player.Id,
                        UserName = history.Player.UserName
                    }
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: CareHistory/PlayerPet/{playerPetId} - Get care history for a specific pet
        [HttpGet("PlayerPet/{playerPetId}")]
        public ActionResult<IEnumerable<object>> GetByPlayerPetId(int playerPetId)
        {
            try
            {
                var playerPet = _context.PlayerPets.Find(playerPetId);
                if (playerPet == null)
                {
                    return NotFound("Player pet not found");
                }
                
                var history = _context.CareHistories
                    .Include(h => h.Activity)
                    .Include(h => h.Player)
                    .Where(h => h.PlayerPetId == playerPetId)
                    .OrderByDescending(h => h.PerformedAt)
                    .Select(h => new
                    {
                        CareHistoryId = h.CareHistoryId,
                        PlayerPetId = h.PlayerPetId,
                        PlayerId = h.PlayerId,
                        ActivityId = h.ActivityId,
                        PerformedAt = h.PerformedAt,
                        ActivityInfo = new
                        {
                            ActivityId = h.Activity.ActivityId,
                            ActivityType = h.Activity.ActivityType
                        },
                        PlayerInfo = new
                        {
                            Id = h.Player.Id,
                            UserName = h.Player.UserName
                        }
                    })
                    .ToList();
                    
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: CareHistory/Player/{playerId} - Get all care history for a player
        [HttpGet("Player/{playerId}")]
        public ActionResult<IEnumerable<object>> GetByPlayerId(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var history = _context.CareHistories
                    .Include(h => h.PlayerPet)
                    .Include(h => h.Activity)
                    .Where(h => h.PlayerId == playerId)
                    .OrderByDescending(h => h.PerformedAt)
                    .Select(h => new
                    {
                        CareHistoryId = h.CareHistoryId,
                        PlayerPetId = h.PlayerPetId,
                        PlayerId = h.PlayerId,
                        ActivityId = h.ActivityId,
                        PerformedAt = h.PerformedAt,
                        PlayerPetInfo = new
                        {
                            PlayerPetId = h.PlayerPet.PlayerPetId,
                            PetCustomName = h.PlayerPet.PetCustomName,
                            PetId = h.PlayerPet.PetId
                        },
                        ActivityInfo = new
                        {
                            ActivityId = h.Activity.ActivityId,
                            ActivityType = h.Activity.ActivityType
                        }
                    })
                    .ToList();
                    
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: CareHistory - Create a new care history record
        [HttpPost]
        public ActionResult<CareHistory> Create(int playerPetId, int playerId, int activityId)
        {
            // Validate player pet exists
            var playerPet = _context.PlayerPets.Find(playerPetId);
            if (playerPet == null)
            {
                return BadRequest("Player pet not found");
            }
            
            // Validate player exists
            var player = _context.Users.Find(playerId);
            if (player == null)
            {
                return BadRequest("Player not found");
            }
            
            // Validate player owns the pet
            if (playerPet.PlayerId != playerId)
            {
                return BadRequest("This pet does not belong to the specified player");
            }
            
            // Validate activity exists
            var activity = _context.CareActivities.Find(activityId);
            if (activity == null)
            {
                return BadRequest("Care activity not found");
            }
            
            var careHistory = new CareHistory
            {
                PlayerPetId = playerPetId,
                PlayerId = playerId,
                ActivityId = activityId,
                PerformedAt = DateTime.Now
            };
            
            _context.CareHistories.Add(careHistory);
            _context.SaveChanges();
            
            // Update pet status based on the activity
            playerPet.LastStatusUpdate = DateTime.Now;
            _context.PlayerPets.Update(playerPet);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = careHistory.CareHistoryId }, careHistory);
        }
        
        // DELETE: CareHistory/{id} - Delete a care history record
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var history = _context.CareHistories.Find(id);
            
            if (history == null)
            {
                return NotFound();
            }
            
            _context.CareHistories.Remove(history);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
