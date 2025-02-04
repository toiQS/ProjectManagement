using PM.Domain;
using PM.Persistence.Context;

namespace RootAPIServices.SeedData
{
    public static class StatusSeeder
    {
        public static async Task Initialize(this IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Status.Any()) return;
            var arr = new[] { "not selected", "waiting", "in progress", "completed early", "finished on time", "behind schedule", "finished late" };
            foreach (var item in arr)
            {
                context.AddRange(new Status
                {
                    Value = item,
                });
            }
            await context.SaveChangesAsync();
        }
    }
}
