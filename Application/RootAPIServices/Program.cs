using Microsoft.EntityFrameworkCore;
using PM.Infrastructure.Configurations;
using PM.Persistence.Configurations;
using PM.Persistence.Context;
using RootAPIServices.SeedData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnectString"));
});

// Add services to the container.
builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
builder.Services.AddThirdPartyServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.Initialize(services);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
