using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Logic;
using PM.Infrastructure.Configurations;
using PM.Persistence.Configurations;
using PM.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectString"));
});


builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
builder.Services.AddThirdPartyServices(builder.Configuration);

builder.Services.AddScoped<IUserLogic, UserLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
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
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        Console.WriteLine(ex.StackTrace?.ReplaceLineEndings());
    }
}
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var email = "user1@user1.com";
    var name = "user";
    var password = "User@1234";
    try
    {
        var identityUser = new ApplicationUser
        {
            Id = "user-1",
            UserName = name,
            Email = email,
            EmailConfirmed = false,
        };
        await userManager.CreateAsync(identityUser, password);
        await userManager.AddToRoleAsync(identityUser, "User");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        Console.WriteLine(ex.StackTrace?.ReplaceLineEndings());
    }
}
app.Run();
