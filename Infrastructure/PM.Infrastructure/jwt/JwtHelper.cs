using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PM.Infrastructure.jwt
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _conConfig;
        public JwtHelper(IConfiguration conConfig)
        {
            _conConfig = conConfig;
        }
        public string GenerateTokenString(string email, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role)) return "Email and Role are requested";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email , email),
                new Claim(ClaimTypes.Role , role),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conConfig["Jwt:SecretKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: _conConfig["Jwt:Issuer"],
                audience: _conConfig["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
