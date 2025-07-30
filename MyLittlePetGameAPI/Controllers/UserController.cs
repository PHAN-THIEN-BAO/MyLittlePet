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
        }
        
        // GET: User/login - Get user by username and password
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
            
            // Return the full user object for all roles including Player
            return Ok(user);
        }
        
        // POST: User/register - Register a new player
        [HttpPost("register")]
        public ActionResult<User> RegisterPlayer(string userName, string password,
            int? coin = 100, int? diamond = 0, int? gem = 0)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Username and password are required");
                }
                
                // Trim whitespace from inputs
                userName = userName.Trim();
                password = password.Trim();
                
                // Additional validation
                if (userName.Length > 100)
                {
                    return BadRequest("Username cannot exceed 100 characters");
                }
                
                if (password.Length > 100)
                {
                    return BadRequest("Password cannot exceed 100 characters");
                }
                
                // Check if username already exists
                if (_context.Users.Any(u => u.UserName == userName))
                {
                    return BadRequest("Username already exists");
                }
                
                var newPlayer = new User
                {
                    Role = "Player", // Always set role to Player for this endpoint
                    UserName = userName,
                    Password = password,
                    Email = null, // Generate unique placeholder email
                    Level = 1,
                    Coin = coin ?? 100, // Default starting coins
                    Diamond = diamond ?? 0,
                    Gem = gem ?? 0,
                    Position = null, // Initialize Position field
                    Exp = 0, // Initialize EXP field
                    JoinDate = DateTime.Now
                };
                
                // Add some debugging
                Console.WriteLine($"Creating user: {newPlayer.UserName}, Role: {newPlayer.Role}");
                
                _context.Users.Add(newPlayer);
                
                // Save and get more detailed error info
                var result = _context.SaveChanges();
                Console.WriteLine($"SaveChanges result: {result}");
                
                // Return only the ID for security
                return Ok(new { PlayerId = newPlayer.Id, Message = "Registration successful" });
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                var stackTrace = ex.StackTrace ?? "No stack trace";
                
                return StatusCode(500, new { 
                    Message = "An error occurred during registration", 
                    Error = ex.Message,
                    InnerException = innerException,
                    StackTrace = stackTrace
                });
            }
        }
        
        // POST: User - Create a new user
        [HttpPost]
        public ActionResult<User> Create(string role, string userName, string password, string? email, 
            int? level, int? coin, int? diamond, int? gem, int? exp)
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
                Level = level ?? 1, // Default to level 1 if not provided
                Coin = coin ?? 0, // Default to 0 if not provided
                Diamond = diamond ?? 0, // Default to 0 if not provided
                Gem = gem ?? 0, // Default to 0 if not provided
                Position = null, // Initialize Position field
                Exp = exp ?? 0, // Default to 0 if not provided
                JoinDate = DateTime.Now
            };
            
            _context.Users.Add(newUser);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);   
        }
        
        // PUT: User/{id} - Update an existing user
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, string? role, string? userName, string? password, 
            string? email, int? level, int? coin, int? diamond, int? gem, int? exp)
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
            
            if (exp.HasValue)
            {
                user.Exp = exp;
            }
            
            _context.Users.Update(user);
            _context.SaveChanges();
            
            return Ok(user);
        }
        
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
        
        // PUT: User/{id}/Experience - Update user experience
        [HttpPut("{id}/Experience")]
        public IActionResult UpdateUserExperience(int id, [FromBody] int exp)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            user.Exp = exp;
            _context.SaveChanges();
            
            return Ok(new { message = "Experience updated successfully", newExp = exp });
        }
        
        // PUT: User/{id}/Position - Update user position
        [HttpPut("{id}/Position")]
        public IActionResult UpdateUserPosition(int id, [FromBody] float position)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            user.Position = position;
            _context.SaveChanges();
            
            return Ok(new { message = "Position updated successfully", newPosition = position });
        }
        
        // GET: User/{id}/Stats - Get comprehensive user statistics
        [HttpGet("{id}/Stats")]
        public ActionResult GetUserStats(int id)
        {
            var user = _context.Users.Find(id);
            
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            var petCount = _context.PlayerPets.Count(pp => pp.PlayerId == id);
            var inventoryCount = _context.PlayerInventories.Sum(pi => pi.Quantity ?? 0);
            var achievementCount = _context.PlayerAchievements.Count(pa => pa.PlayerId == id);
            var collectedAchievements = _context.PlayerAchievements.Count(pa => pa.PlayerId == id && pa.IsCollected == true);
            var totalGameScore = _context.GameRecords.Where(gr => gr.PlayerId == id).Sum(gr => gr.Score ?? 0);
            var careActivities = _context.CareHistories.Count(ch => ch.PlayerId == id);
            
            var stats = new
            {
                UserId = user.Id,
                UserName = user.UserName,
                Level = user.Level,
                Experience = user.Exp,
                Position = user.Position,
                Coins = user.Coin,
                Diamonds = user.Diamond,
                Gems = user.Gem,
                PetCount = petCount,
                InventoryItemCount = inventoryCount,
                TotalAchievements = achievementCount,
                CollectedAchievements = collectedAchievements,
                TotalGameScore = totalGameScore,
                CareActivitiesPerformed = careActivities,
                JoinDate = user.JoinDate
            };
            
            return Ok(stats);
        }
        
        // GET: User/search - Search for players by name (case-insensitive partial match)
        [HttpGet("search")]
        public ActionResult SearchPlayers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }
            
            // Search for players (role = "Player") whose username contains the search term (case-insensitive)
            var players = _context.Users
                .Where(u => u.Role == "Player" && 
                           u.UserName != null && 
                           u.UserName.ToLower().Contains(searchTerm.ToLower()))
                .Select(u => new 
                {
                    u.Id,
                    u.UserName,
                    u.Level,
                    u.Coin,
                    u.Diamond,
                    u.Gem,
                    u.JoinDate,
                    u.Exp,
                    u.Position
                })
                .ToList();
            
            if (!players.Any())
            {
                return Ok(new { Message = "No players found", Players = new List<object>() });
            }
            
            return Ok(new { Message = $"Found {players.Count} player(s)", Players = players });
        }
    }
}
