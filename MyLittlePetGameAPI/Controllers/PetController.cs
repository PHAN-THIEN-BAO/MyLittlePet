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
        
        // POST: Pet - Create a new pet
        [HttpPost]
        public ActionResult<Pet> Create(int? adminId, string petType, string petDefaultName, string? description)
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
                Description = description
            };
            
            _context.Pets.Add(newPet);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = newPet.PetId }, newPet);
        }
        
        // PUT: Pet/{id} - Update a pet
        [HttpPut("{id}")]
        public ActionResult<Pet> Update(int id, int? adminId, string petType, string petDefaultName, string? description)
        {
            var pet = _context.Pets.Find(id);
            
            if (pet == null)
            {
                return NotFound();
            }
            
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
            
            pet.AdminId = adminId;
            pet.PetType = petType;
            pet.PetDefaultName = petDefaultName;
            pet.Description = description;
            
            _context.Pets.Update(pet);
            _context.SaveChanges();
            
            return Ok(pet);
        }
        
        // DELETE: Pet/{id} - Delete a pet
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var pet = _context.Pets.Find(id);
            
            if (pet == null)
            {
                return NotFound();
            }
            
            _context.Pets.Remove(pet);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
