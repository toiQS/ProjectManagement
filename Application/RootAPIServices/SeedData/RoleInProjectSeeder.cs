using PM.Domain;
using PM.Persistence.Context;

namespace RootAPIServices.SeedData
{
    public static class RoleInProjectSeeder
    {
        public static async Task Initialize( IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if(context.RoleInProject.Any()) return;
            context.AddRange(
                new RoleInProject()
                {
                    RoleName = "Owner",
                    Id = $"1002-123456-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Minute}-{DateTime.Now.Ticks}",
                    RoleDescription = ""
                },
                new RoleInProject()
                {
                    RoleName = "Leader",
                    Id = $"1002-123456-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.AddMinutes(1).Minute}-{DateTime.Now.Minute}-{DateTime.Now.Ticks}}",
                    RoleDescription = ""
                }, new RoleInProject()
                {
                    RoleName = "Manager",
                    Id = $"1002-123456-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.AddMinutes(2).Minute}-{DateTime.Now.Minute}-{DateTime.Now.Ticks}",
                    RoleDescription = ""
                }, new RoleInProject()
                {
                    RoleName = "Member",
                    Id = $"1002-123456-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.AddMinutes(3).Minute}-{DateTime.Now.Minute}-{DateTime.Now.Ticks}",
                    RoleDescription = ""
                }
                );
            await context.SaveChangesAsync();
        }
    }
}
