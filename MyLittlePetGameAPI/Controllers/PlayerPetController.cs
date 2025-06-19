using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerPetController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public PlayerPetController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: PlayerPet - Get all player pets
        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var playerPets = _context.PlayerPets
                    .Include(pp => pp.Player)
                    .Include(pp => pp.Pet)
                    .Select(pp => new
                    {
                        PlayerPetId = pp.PlayerPetId,
                        PlayerId = pp.PlayerId,
                        PetId = pp.PetId,
                        PetCustomName = pp.PetCustomName,
                        AdoptedAt = pp.AdoptedAt,
                        Level = pp.Level,
                        Status = pp.Status,
                        LastStatusUpdate = pp.LastStatusUpdate,
                        PetInfo = new
                        {
                            PetId = pp.Pet.PetId,
                            PetType = pp.Pet.PetType,
                            PetDefaultName = pp.Pet.PetDefaultName,
                            Description = pp.Pet.Description
                        },
                        PlayerInfo = new
                        {
                            Id = pp.Player.Id,
                            UserName = pp.Player.UserName
                        }
                    })
                    .ToList();
                
                return Ok(playerPets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerPet/{id} - Get player pet by ID
        [HttpGet("{id}")]
        public ActionResult<object> GetById(int id)
        {
            try
            {
                var playerPet = _context.PlayerPets
                    .Include(pp => pp.Player)
                    .Include(pp => pp.Pet)
                    .FirstOrDefault(pp => pp.PlayerPetId == id);
                
                if (playerPet == null)
                {
                    return NotFound();
                }
                
                var result = new
                {
                    PlayerPetId = playerPet.PlayerPetId,
                    PlayerId = playerPet.PlayerId,
                    PetId = playerPet.PetId,
                    PetCustomName = playerPet.PetCustomName,
                    AdoptedAt = playerPet.AdoptedAt,
                    Level = playerPet.Level,
                    Status = playerPet.Status,
                    LastStatusUpdate = playerPet.LastStatusUpdate,
                    PetInfo = new
                    {
                        PetId = playerPet.Pet.PetId,
                        PetType = playerPet.Pet.PetType,
                        PetDefaultName = playerPet.Pet.PetDefaultName,
                        Description = playerPet.Pet.Description
                    },
                    PlayerInfo = new
                    {
                        Id = playerPet.Player.Id,
                        UserName = playerPet.Player.UserName
                    }
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerPet/Player/{playerId} - Get all pets for a player
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
                
                var playerPets = _context.PlayerPets
                    .Include(pp => pp.Pet)
                    .Where(pp => pp.PlayerId == playerId)
                    .Select(pp => new
                    {
                        PlayerPetId = pp.PlayerPetId,
                        PlayerId = pp.PlayerId,
                        PetId = pp.PetId,
                        PetCustomName = pp.PetCustomName,
                        AdoptedAt = pp.AdoptedAt,
                        Level = pp.Level,
                        Status = pp.Status,
                        LastStatusUpdate = pp.LastStatusUpdate,
                        PetInfo = new
                        {
                            PetId = pp.Pet.PetId,
                            PetType = pp.Pet.PetType,
                            PetDefaultName = pp.Pet.PetDefaultName,
                            Description = pp.Pet.Description
                        }
                    })
                    .ToList();
                    
                return Ok(playerPets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: PlayerPet - Adopt a pet for a player
        [HttpPost]
        public ActionResult<PlayerPet> Create(int playerId, int petId, string? petCustomName, string? status)
        {
            // Validate player exists
            var player = _context.Users.Find(playerId);
            if (player == null)
            {
                return BadRequest("Player not found");
            }
            
            // Validate pet exists
            var pet = _context.Pets.Find(petId);
            if (pet == null)
            {
                return BadRequest("Pet not found");
            }
            
            // Check if custom name is already used by this player
            if (!string.IsNullOrEmpty(petCustomName) && 
                _context.PlayerPets.Any(pp => pp.PlayerId == playerId && pp.PetCustomName == petCustomName))
            {
                return BadRequest("Pet name already used by this player");
            }
            
            var playerPet = new PlayerPet
            {
                PlayerId = playerId,
                PetId = petId,
                PetCustomName = petCustomName,
                AdoptedAt = DateTime.Now,
                Level = 1,
                Status = status,
                LastStatusUpdate = DateTime.Now
            };
            
            _context.PlayerPets.Add(playerPet);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = playerPet.PlayerPetId }, playerPet);
        }
        
        // PUT: PlayerPet/{id} - Update a player pet
        [HttpPut("{id}")]
        public ActionResult<PlayerPet> Update(int id, string? petCustomName, int? level, string? status)
        {
            var playerPet = _context.PlayerPets.Find(id);
            
            if (playerPet == null)
            {
                return NotFound();
            }
            
            // Check if custom name is already used by this player (if name is being changed)
            if (!string.IsNullOrEmpty(petCustomName) && 
                petCustomName != playerPet.PetCustomName &&
                _context.PlayerPets.Any(pp => pp.PlayerId == playerPet.PlayerId && pp.PetCustomName == petCustomName))
            {
                return BadRequest("Pet name already used by this player");
            }
            
            if (!string.IsNullOrEmpty(petCustomName))
            {
                playerPet.PetCustomName = petCustomName;
            }
            
            if (level.HasValue)
            {
                playerPet.Level = level;
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                playerPet.Status = status;
                playerPet.LastStatusUpdate = DateTime.Now;
            }
            
            _context.PlayerPets.Update(playerPet);
            _context.SaveChanges();
            
            return Ok(playerPet);
        }
        
        // DELETE: PlayerPet/{id} - Delete a player pet
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var playerPet = _context.PlayerPets.Find(id);
            
            if (playerPet == null)
            {
                return NotFound();
            }
            
            _context.PlayerPets.Remove(playerPet);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
