using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Services;
using System.Security.Claims;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(RegisterUserDTO registerUserDTO)
        {
            try
            {
                var response = await _authService.Register(registerUserDTO);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDTO)
        {
            try
            {
                var response = await _authService.Login(loginDTO);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<string>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || emailClaim == null || roleClaim == null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                UserId = userIdClaim.Value,
                Email = emailClaim.Value,
                Role = roleClaim.Value
            });
        }
    }
}
