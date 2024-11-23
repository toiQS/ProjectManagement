using PM.DomainServices.ILogic;
using PM.DomainServices.Logic;
using PM.Infrastructure.Configurations;
using PM.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.Run();
