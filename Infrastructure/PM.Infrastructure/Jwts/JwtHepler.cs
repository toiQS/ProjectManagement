using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PM.Domain;
using Shared.appUser;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PM.Infrastructure.Jwts
{
    /// <summary>
    /// Helper class for generating and validating JSON Web Tokens (JWT).
    /// </summary>
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtHelper"/> class.
        /// </summary>
        /// <param name="configuration">Configuration for retrieving JWT settings.</param>
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(ApplicationUser appUser,string role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim (ClaimTypes.Email, appUser.Email),
                    new Claim(ClaimTypes.Role, role)
                };
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                var tokenOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signingCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            }
            catch
            {
                throw;
            }
        }
        
    }
}
