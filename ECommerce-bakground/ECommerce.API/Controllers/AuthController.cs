using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(CreateUserDto createUserDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(createUserDto);
                if (result == null)
                    return BadRequest("Registration failed");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (result == null)
                return Unauthorized("Invalid refresh token");

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.LogoutAsync(refreshTokenDto.RefreshToken);
            if (!result)
                return BadRequest("Invalid refresh token");

            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<ActionResult> LogoutAll()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return BadRequest("Invalid user");

            var result = await _authService.LogoutAllAsync(userId);
            return Ok(new { message = "Logout from all devices successful" });
        }
    }
}
