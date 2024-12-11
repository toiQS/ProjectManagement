﻿using Microsoft.Extensions.DependencyInjection;
using PM.DomainServices.ILogic;
using PM.DomainServices.Logic;

namespace PM.DomainServices
{
    public static class Configurations
    {
        public static void Initialize(this IServiceCollection services)
        {
            RegisterLogic(services);
        }
        private static void RegisterLogic(IServiceCollection services)
        {
            services.AddScoped<IAuthLogic, AuthLogic>();
            services.AddScoped<IMemberLogic, MemberLogic>();
            services.AddScoped<IPlanLogic, PlanLogic>();
            services.AddScoped<IPositionLogic, PositionLogic>();
            services.AddScoped<IProjectLogic, ProjectLogic>();
            services.AddScoped<ITaskLogic, TaskLogic>();
        }
    }
}
