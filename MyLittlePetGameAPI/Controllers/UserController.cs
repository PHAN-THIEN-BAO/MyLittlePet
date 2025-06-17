using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public UserController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: User - Get all users
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return Ok(_context.Users.ToList());
        }
          // GET: User/{id} - Get user by ID
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }
        
        // GET: User/{id}/PetCount - Get number of pets owned by user
        [HttpGet("{id}/PetCount")]
        public ActionResult<int> GetPetCount(int id)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            var petCount = _context.PlayerPets.Count(pp => pp.PlayerId == id);
            
            return Ok(new { UserId = id, PetCount = petCount });
        }        // GET: User/login - Get user by username and password
        [HttpGet("login")]
        public ActionResult GetByLogin(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required");
            }
            
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName && u.Password == password);
            
            if (user == null)
            {
                return NotFound("User not found");
            }
              // If role is Player, only return the User ID
            if (user.Role == "Player")
            {
                return Ok(new { UserId = user.Id });
            }
            
            // For other roles, return the full user object
            return Ok(user);
        }
        
        // POST: User/register - Register a new player
        [HttpPost("register")]
        public ActionResult<User> RegisterPlayer(string userName, string email, string password, 
            int? coin = 100, int? diamond = 0, int? gem = 0)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Username, email, and password are required");
            }
            
            // Check if username already exists
            if (_context.Users.Any(u => u.UserName == userName))
            {
                return BadRequest("Username already exists");
            }
            
            // Check if email already exists
            if (_context.Users.Any(u => u.Email == email))
            {
                return BadRequest("Email already exists");
            }
            
            var newPlayer = new User
            {
                Role = "Player", // Always set role to Player for this endpoint
                UserName = userName,
                Password = password,
                Email = email,
                UserStatus = "ACTIVE",
                Level = 1,
                Coin = coin ?? 100, // Default starting coins
                Diamond = diamond ?? 0,
                Gem = gem ?? 0,
                JoinDate = DateTime.Now
            };
            
            _context.Users.Add(newPlayer);
            _context.SaveChanges();
            
            // Return only the ID for security
            return Ok(new { PlayerId = newPlayer.Id, Message = "Registration successful" });
        }
        
        // POST: User - Create a new user
        [HttpPost]
        public ActionResult<User> Create(string role, string userName, string password, string? email, 
            string? userStatus, int? level, int? coin, int? diamond, int? gem)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Role, username, and password are required");
            }
            
            // Check if username already exists
            if (_context.Users.Any(u => u.UserName == userName))
            {
                return BadRequest("Username already exists");
            }
            
            // Check if email already exists (if provided)
            if (!string.IsNullOrEmpty(email) && _context.Users.Any(u => u.Email == email))
            {
                return BadRequest("Email already exists");
            }
            
            var newUser = new User
            {
                Role = role,
                UserName = userName,
                Password = password,
                Email = email,
                UserStatus = userStatus ?? "ACTIVE", // Default to ACTIVE if not provided
                Level = level ?? 1, // Default to level 1 if not provided
                Coin = coin ?? 0, // Default to 0 if not provided
                Diamond = diamond ?? 0, // Default to 0 if not provided
                Gem = gem ?? 0, // Default to 0 if not provided
                JoinDate = DateTime.Now
            };
            
            _context.Users.Add(newUser);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);   
        }
        
        // PUT: User/{id} - Update an existing user
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, string? role, string? userName, string? password, 
            string? email, int? level, int? coin, int? diamond, int? gem, string? userStatus)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound();
            }
            
            // Check if username is being changed and already exists
            if (!string.IsNullOrEmpty(userName) && userName != user.UserName && 
                _context.Users.Any(u => u.UserName == userName))
            {
                return BadRequest("Username already exists");
            }
            
            // Check if email is being changed and already exists
            if (!string.IsNullOrEmpty(email) && email != user.Email && 
                _context.Users.Any(u => u.Email == email))
            {
                return BadRequest("Email already exists");
            }
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(role))
            {
                user.Role = role;
            }
            
            if (!string.IsNullOrEmpty(userName))
            {
                user.UserName = userName;
            }
            
            if (!string.IsNullOrEmpty(password))
            {
                user.Password = password;
            }
            
            if (email != null) // Allow setting email to null
            {
                user.Email = email;
            }
            
            if (!string.IsNullOrEmpty(userStatus))
            {
                user.UserStatus = userStatus;
            }
            
            if (level.HasValue)
            {
                user.Level = level;
            }
            
            if (coin.HasValue)
            {
                user.Coin = coin;
            }
            
            if (diamond.HasValue)
            {
                user.Diamond = diamond;
            }
            
            if (gem.HasValue)
            {
                user.Gem = gem;
            }
            
            _context.Users.Update(user);
            _context.SaveChanges();
            
            return Ok(user);        }
        
        // DELETE: User/{id} - Delete a user
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound();
            }
            
            // Check for related records that would prevent deletion
            var hasPlayerPets = _context.PlayerPets.Any(pp => pp.PlayerId == id);
            var hasInventory = _context.PlayerInventories.Any(pi => pi.PlayerId == id);
            var hasAchievements = _context.PlayerAchievements.Any(pa => pa.PlayerId == id);
            var hasGameRecords = _context.GameRecords.Any(gr => gr.PlayerId == id);
            var hasCareHistory = _context.CareHistories.Any(ch => ch.PlayerId == id);
            var isAdmin = _context.Pets.Any(p => p.AdminId == id) || _context.ShopProducts.Any(sp => sp.AdminId == id);
            
            if (hasPlayerPets || hasInventory || hasAchievements || hasGameRecords || hasCareHistory || isAdmin)
            {
                return BadRequest("Cannot delete user with existing related records. Delete related records first.");
            }
            
            _context.Users.Remove(user);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
