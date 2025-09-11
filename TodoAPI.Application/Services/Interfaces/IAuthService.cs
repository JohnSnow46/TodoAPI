using TodoAPI.Application.DTOs.Common;
using TodoAPI.Application.DTOs.User;

namespace TodoAPI.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<bool>> LogoutAsync(string refreshToken);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
