using PM.DomainServices;
using PM.Infrastructure.Configurations;
using PM.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
builder.Services.AddThirdPartyServices(builder.Configuration);
builder.Services.AddInitialize(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
