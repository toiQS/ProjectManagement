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

        #region Generate Token

        /// <summary>
        /// Generates a JWT token based on the user's email and role.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public string GenerateToken(string email, string role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
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
            catch (Exception ex)
            {
                // Log exception (optional: use a logging framework)
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

        #region Parse Token

        /// <summary>
        /// Parses a JWT token and extracts the user's email and role.
        /// </summary>
        /// <param name="token">The JWT token to parse.</param>
        /// <returns>A <see cref="UserResult"/> object containing the email and role.</returns>
        public UserResult ParseToken(string token)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true // Validate token expiration
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                return new UserResult
                {
                    Email = email,
                    Role = role
                };
            }
            catch (Exception ex)
            {
                // Log exception (optional: use a logging framework)
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion
    }
}
