using PM.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
