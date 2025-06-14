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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = users.Select(user => new UserDTO
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
            }).ToList();

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to access their own data unless they're an admin
            if (userId != id.ToString() && userRole != "Admin")
            {
                return Forbid();
            }

            var userDto = new UserDTO
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

            return userDto;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to update their own data unless they're an admin
            if (userId != id.ToString() && userRole != "Admin")
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(updateUserDto.UserName))
            {
                user.UserName = updateUserDto.UserName;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                // Check if the email is already taken
                if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.ID != id))
                {
                    return BadRequest("Email already exists");
                }
                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

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

        [HttpPut("{id}/currency")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserCurrency(int id, [FromBody] Dictionary<string, int> currencyUpdate)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (currencyUpdate.ContainsKey("coin"))
            {
                user.Coin = currencyUpdate["coin"];
            }

            if (currencyUpdate.ContainsKey("diamond"))
            {
                user.Diamond = currencyUpdate["diamond"];
            }

            if (currencyUpdate.ContainsKey("gem"))
            {
                user.Gem = currencyUpdate["gem"];
            }

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
