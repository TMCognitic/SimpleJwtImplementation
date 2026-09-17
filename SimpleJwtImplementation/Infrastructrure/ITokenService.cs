using SimpleJwtImplementation.Models;

namespace SimpleJwtImplementation.Infrastructrure
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        int GetUserIdFromToken(string token);
    }
}