using PM.Domain;
using System.Security.Claims;

namespace PM.Infrastructure.Jwts
{
    public interface IJwtHelper
    {
        public string GenerateToken(string email, string role);
    }
}