using System.Security.Claims;

namespace PM.Infrastructure.Jwts
{
    public interface IJwtHelper
    {
        public string GenerateJwtToken(string userId, string userRole);
        public ClaimsPrincipal ValidateJwtToken(string token);
    }
}