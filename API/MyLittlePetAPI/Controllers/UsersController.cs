using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.Models;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyLittlePetDbContext _context;

        public UsersController(MyLittlePetDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.ID }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
            try
            {
                // Find user by username or email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == loginRequest.Username || u.Email == loginRequest.Username);

                if (user == null)
                {
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    });
                }

                // In a real application, you should hash passwords and compare hashes
                // For now, we'll do a simple comparison
                if (user.Password != loginRequest.Password)
                {
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    });
                }

                // Login successful - remove password from response for security
                var userResponse = new User
                {
                    ID = user.ID,
                    Role = user.Role,
                    UserName = user.UserName,
                    Email = user.Email,
                    Level = user.Level,
                    Coin = user.Coin,
                    Diamond = user.Diamond,
                    Gem = user.Gem,
                    JoinDate = user.JoinDate
                    // Note: Password is intentionally omitted for security
                };

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    User = userResponse,
                    Token = GenerateSimpleToken(user.ID) // Simple token generation
                });
            }
            catch (Exception ex)
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(User user)
        {
            try
            {
                // Check if username already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == user.UserName || u.Email == user.Email);

                if (existingUser != null)
                {
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Username or email already exists"
                    });
                }

                // Set default values
                user.JoinDate = DateTime.Now;
                user.Level = 1;
                user.Diamond = 0;
                user.Gem = 0;
                user.Coin = 100; // Starting coins

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Return success response without password
                var userResponse = new User
                {
                    ID = user.ID,
                    Role = user.Role,
                    UserName = user.UserName,
                    Email = user.Email,
                    Level = user.Level,
                    Coin = user.Coin,
                    Diamond = user.Diamond,
                    Gem = user.Gem,
                    JoinDate = user.JoinDate
                };

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    User = userResponse,
                    Token = GenerateSimpleToken(user.ID)
                });
            }
            catch (Exception ex)
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during registration"
                });
            }
        }

        private string GenerateSimpleToken(int userId)
        {
            // This is a very simple token generation for demonstration
            // In production, use proper JWT tokens or similar
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"user_{userId}_{DateTime.UtcNow.Ticks}"));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
