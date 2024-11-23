using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
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

        /// <summary>
        /// Generates a JWT token with user-specific claims.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="userRole">The user's role.</param>
        /// <returns>A string representing the generated JWT token.</returns>
        public string GenerateJwtToken(string userId, string userRole)
        {
            // Retrieve the secret key and create signing credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define the claims included in the JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, userRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique identifier for the token
            };

            // Create the token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Set token expiration
                signingCredentials: credentials);

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token and retrieves the associated claims principal.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> if the token is valid; otherwise, null.</returns>
        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);

            // Define the token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // Ensure the token's signature is valid
                IssuerSigningKey = new SymmetricSecurityKey(key), // Use the configured secret key
                ValidateIssuer = true, // Ensure the issuer matches
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true, // Ensure the audience matches
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true, // Ensure the token has not expired
                ClockSkew = TimeSpan.Zero // No additional time window for token validity
            };

            try
            {
                // Validate the token and return the claims principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                // Return null if the token is invalid
                return null;
            }
        }
    }
}
