using TodoAPI.Application.DTOs.Common;
using TodoAPI.Core.Entities;

namespace TodoAPI.Application.Services.Interfaces
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
