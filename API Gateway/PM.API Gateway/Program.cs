using Microsoft.AspNetCore.Identity;
using PM.Domain.DTOs;
using PM.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

    }

}
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var email = "admin@admin.com";
    var name = "Admin";
    var password = "Admin@1234";
    try
    {
        var identityUser = new ApplicationUser
        {
            Id = "user-default",
            UserName = name,
            Email = email,
            EmailConfirmed = false,
        };
        await userManager.CreateAsync(identityUser, password);
        await userManager.AddToRoleAsync(identityUser, "Admin");
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.ToString());
        Console.WriteLine(ex.StackTrace.ReplaceLineEndings());
    }
}

app.Run();
