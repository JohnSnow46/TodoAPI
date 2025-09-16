using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.DTOs.Common;
using TodoAPI.Application.DTOs.User;
using TodoAPI.Application.Services.Interfaces;

namespace TodoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
                }

                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return CreatedAtAction(nameof(Register), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
                }

                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Logout user
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _authService.LogoutAsync(refreshTokenDto.RefreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Get current user info
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(ApiResponse<UserDto>.ErrorResult("Invalid token"));
                }

                // This would need IUserRepository injected or IUserService
                // For now, return a placeholder
                return Ok(ApiResponse<UserDto>.SuccessResult(new UserDto
                {
                    Id = userId,
                    Email = User.FindFirst("email")?.Value ?? "",
                    FirstName = User.FindFirst("firstName")?.Value ?? "",
                    LastName = User.FindFirst("lastName")?.Value ?? ""
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Internal server error"));
            }
        }
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}