using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.Services.Interfaces;
using TodoAPI.Core.Interfaces;

namespace TodoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JwtTestController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JwtTestController> _logger;

        public JwtTestController(IJwtService jwtService, IUnitOfWork unitOfWork, ILogger<JwtTestController> logger)
        {
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Test JWT token generation with first user in database
        /// </summary>
        [HttpPost("generate-token")]
        public async Task<ActionResult> GenerateTestToken()
        {
            try
            {
                // Get first user from database
                var users = await _unitOfWork.Users.GetAllUsersAsync();
                var firstUser = users.FirstOrDefault();

                if (firstUser == null)
                {
                    return BadRequest("No users found. Create a user first using /api/test/test-user");
                }

                // Generate token
                var token = _jwtService.GenerateToken(firstUser);
                var authResponse = _jwtService.GenerateAuthResponse(firstUser);

                return Ok(new
                {
                    Message = "JWT Token generated successfully!",
                    Token = token,
                    AuthResponse = authResponse,
                    User = new
                    {
                        firstUser.Id,
                        firstUser.Email,
                        firstUser.FirstName,
                        firstUser.LastName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Test JWT token validation
        /// </summary>
        [HttpPost("validate-token")]
        public ActionResult ValidateToken([FromBody] TokenRequest request)
        {
            try
            {
                var isValid = _jwtService.ValidateToken(request.Token);
                var userId = _jwtService.GetUserIdFromToken(request.Token);

                return Ok(new
                {
                    IsValid = isValid,
                    UserId = userId,
                    Message = isValid ? "Token is valid!" : "Token is invalid!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }

    public class TokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}