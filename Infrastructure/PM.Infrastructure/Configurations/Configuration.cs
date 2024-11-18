using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PM.Infrastructure.Configurations
{
    public static class Configuration
    {
        public static void AddThirdPartyServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddJwtServices(services, configuration);
        }
        public static void AddJwtServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateActor = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = configuration.GetSection("Jwt:Audience").Value,
                        ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                        (configuration.GetSection("Jwt:SecretKey").Value))
                    };
                });
        }
    }
}
