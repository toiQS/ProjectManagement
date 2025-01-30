using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PM.DomainServices
{
    public static class Configurations
    {
        public static void AddInitialize(this IServiceCollection services,IConfiguration configuration)
        {
            RegisterServices(services);
            RegisterLogic(services);
        }
        private static void RegisterLogic(IServiceCollection services)
        {
            
        }
        private static void RegisterServices(IServiceCollection services)
        {
            
        }
    }
}
