using TodoAPI.Core.Entities;
using TodoAPI.Application.DTOs.common;

namespace TodoAPI.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        AuthResponse GenerateAuthResponse(User user);
        bool ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
    }
}
