using AutoMapper;
using BCrypt.Net;
using TodoAPI.Application.DTOs.Common;
using TodoAPI.Application.DTOs.User;
using TodoAPI.Application.Services.Interfaces;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Interfaces;

namespace TodoAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return ApiResponse<AuthResponse>.ErrorResult("Invalid email or password");
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponse>.ErrorResult("Invalid email or password");
            }

            var authResponse = _jwtService.GenerateAuthResponse(user);
            return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Login successful");
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponse>.ErrorResult("User with this email already exists");
            }

            // Validate password confirmation
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return ApiResponse<AuthResponse>.ErrorResult("Password and confirmation password do not match");
            }

            // Create new user
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = HashPassword(registerDto.Password);

            var createdUser = await _unitOfWork.Users.AddUserAsync(user);
            var authResponse = _jwtService.GenerateAuthResponse(createdUser);

            return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Registration successful");
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            // TODO store refresh tokens in database
            // For now, return error
            return ApiResponse<AuthResponse>.ErrorResult("Refresh token functionality not implemented yet");
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
        {
            // TODO invalidate the refresh token
            // For now, return success
            return ApiResponse<bool>.SuccessResult(true, "Logout successful");
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}