using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyLittlePetAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> Register(RegisterUserDTO registerUserDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerUserDTO.Email))
            {
                throw new ApplicationException("Email already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password);

            var user = new User
            {
                Role = registerUserDTO.Role,
                UserName = registerUserDTO.UserName,
                Email = registerUserDTO.Email,
                Password = passwordHash,
                Coin = 100, // Default starting coins
                Diamond = 0,
                Gem = 0,
                Level = 1,
                JoinDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null)
            {
                throw new ApplicationException("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                throw new ApplicationException("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                User = MapToUserDto(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private UserDTO MapToUserDto(User user)
        {
            return new UserDTO
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
        }
    }
}
