using Microsoft.EntityFrameworkCore;
using PM.Infrastructure.Configurations;
using PM.Persistence.Configurations;
using PM.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectString"));
//});

builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
builder.Services.AddThirdPartyServices(builder.Configuration);
app.MapGet("/", () => "Hello World!");

app.Run();
