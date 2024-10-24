using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Domain.DTOs;
using PM.Persistence.Context;

namespace PM.Persistence.Configurations
{
    #region configuration services static class
    public static class ConfigurationServices
    {

        public static void ConfigurationAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDatabase(configuration, services);
            AddAuth(services);
            AddServices(services);
        }
        private static void RegisterDatabase(IConfiguration configuration, IServiceCollection services)
        {
            var connectString = "Server=localhost,1433;Database=Blog-Web;Trusted_Connection=True;MultipleActiveResultSets=true;trustServerCertificate=true;User Id=sa;password=Akai@1234";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectString);
            });
        }
        private static void AddAuth(this IServiceCollection services)
        {
            
            services.AddIdentity<ApplicationUser,IdentityRole<string>>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
        }
        private static void AddServices(IServiceCollection services)
        {
            
        }

    }
    #endregion
    #region Configuration has constructor
    public class ConfiguraionService
    {
        public ConfiguraionService(IServiceCollection services, IConfiguration configuration)
        {
            this.RegisterDatabase(services);
        }
        public void RegisterDatabase(IServiceCollection services)
        {
            var connectString = "Server=localhost,1433;Database=Blog-Web;Trusted_Connection=True;MultipleActiveResultSets=true;trustServerCertificate=true;User Id=sa;password=Akai@1234";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectString);
            });
        }
    }
    #endregion
}
