using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Persistence.IServices;
using PM.Persistence.Services;

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
            //services.AddScoped<IAuthLogic, AuthLogic>();
            //services.AddScoped<IMemberLogic, MemberLogic>();
            //services.AddScoped<IPlanLogic, PlanLogic>();
            //services.AddScoped<IPositionLogic, PositionLogic>();
            //services.AddScoped<IProjectLogic, ProjectLogic>();
            //services.AddScoped<ITaskLogic, TaskLogic>();
        }
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IApplicationUserServices, ApplicationUserServices>();
            services.AddScoped<IApplicationUserServices, ApplicationUserServices>();
            services.AddScoped<IMemberInTaskServices, MemberInTaskServices>();
            services.AddScoped<IPlanInProjectServices, PlanInProjectServices>();
            services.AddScoped<IPlanServices, PlanServices>();
            services.AddScoped<IPositionInProjectServices, PositionInProjectServices>();
            services.AddScoped<IPositionWorkOfMemberServices, PositionWorkOfMemberServices>();
            services.AddScoped<IProjectServices, ProjectServices>();
            services.AddScoped<IRoleApplicationUserInProjectServices, RoleApplicationUserInProjectServices>();
            services.AddScoped<IRoleInProjectServices, RoleInProjectServices>();
            services.AddScoped<ITaskInPlanServices, TaskInPlanServices>();
            services.AddScoped<ITaskServices, TaskServices>();
            services.AddScoped<IStatusServices, StatusServices>();
        }
    }
}
