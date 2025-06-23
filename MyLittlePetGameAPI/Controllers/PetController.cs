using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public PetController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: Pet - Get all pets
        [HttpGet]
        public ActionResult<IEnumerable<Pet>> Get()
        {
            return Ok(_context.Pets.ToList());
        }
        
        // GET: Pet/{id} - Get pet by ID
        [HttpGet("{id}")]
        public ActionResult<Pet> GetById(int id)
        {
            var pet = _context.Pets.Find(id);
            
            if (pet == null)
            {
                return NotFound();
            }
            
            return Ok(pet);
        }
          // GET: Pet/Status/{status} - Get pets by status
        [HttpGet("Status/{status}")]
        public ActionResult<IEnumerable<Pet>> GetByStatus(int status)
        {
            var pets = _context.Pets
                .Include(p => p.Admin)
                .Where(p => p.PetStatus == status)
                .ToList();
                
            return Ok(pets);
        }
          // POST: Pet - Create a new pet
        [HttpPost]
        public ActionResult<Pet> Create(int? adminId, string petType, string petDefaultName, string? description, int? petStatus)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(petType) || string.IsNullOrEmpty(petDefaultName))
                {
                    return BadRequest("Pet type and default name are required");
                }
                
                // Validate adminId exists if provided
                if (adminId.HasValue)
                {
                    var admin = _context.Users.Find(adminId);
                    if (admin == null)
                    {
                        return BadRequest("Admin user not found");
                    }
                }
                
                var newPet = new Pet
                {
                    AdminId = adminId,
                    PetType = petType,
                    PetDefaultName = petDefaultName,
                    Description = description,
                    PetStatus = petStatus ?? 1 // Default to 1 (active) if not provided
                };
                
                _context.Pets.Add(newPet);
                _context.SaveChanges();
                
                // Return simplified response to avoid serialization issues
                return Ok(new
                {
                    message = "Pet created successfully",
                    pet = new
                    {
                        petId = newPet.PetId,
                        adminId = newPet.AdminId,
                        petType = newPet.PetType,
                        petDefaultName = newPet.PetDefaultName,
                        description = newPet.Description,
                        petStatus = newPet.PetStatus
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception but return a clean error message
                Console.WriteLine($"Error in Create method: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }        // PUT: Pet/{id} - Update a pet
        [HttpPut("{id}")]
        public ActionResult<Pet> Update(int id, int? adminId, string? petType, string? petDefaultName, string? description, int? petStatus)
        {
            try
            {
                var pet = _context.Pets.Find(id);
                
                if (pet == null)
                {
                    return NotFound();
                }
                
                // Validate required fields if they're being updated
                if ((petType != null && string.IsNullOrEmpty(petType)) || 
                    (petDefaultName != null && string.IsNullOrEmpty(petDefaultName)))
                {
                    return BadRequest("Pet type and default name cannot be empty");
                }
                
                // Validate adminId exists if provided
                if (adminId.HasValue)
                {
                    var admin = _context.Users.Find(adminId);
                    if (admin == null)
                    {
                        return BadRequest("Admin user not found");
                    }
                    pet.AdminId = adminId;
                }
                
                if (!string.IsNullOrEmpty(petType))
                {
                    pet.PetType = petType;
                }
                
                if (!string.IsNullOrEmpty(petDefaultName))
                {
                    pet.PetDefaultName = petDefaultName;
                }
                
                if (description != null) // Allow setting description to null
                {
                    pet.Description = description;
                }
                
                if (petStatus.HasValue)
                {
                    pet.PetStatus = petStatus;
                }
                
                _context.Pets.Update(pet);
                _context.SaveChanges();
                
                // Return simplified response to avoid serialization issues
                return Ok(new
                {
                    message = "Pet updated successfully",
                    pet = new
                    {
                        petId = pet.PetId,
                        adminId = pet.AdminId,
                        petType = pet.PetType,
                        petDefaultName = pet.PetDefaultName,
                        description = pet.Description,
                        petStatus = pet.PetStatus
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception but return a clean error message
                Console.WriteLine($"Error in Update method: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
          // DELETE: Pet/{id} - Delete a pet
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var pet = _context.Pets.Find(id);
                
                if (pet == null)
                {
                    return NotFound();
                }
                
                _context.Pets.Remove(pet);
                _context.SaveChanges();
                
                return Ok(new { message = "Pet deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception but return a clean error message
                Console.WriteLine($"Error in Delete method: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
