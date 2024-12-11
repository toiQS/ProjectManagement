using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PM.Domain;
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

        
    }
}
