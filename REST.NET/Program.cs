/////////////////////
// Configure builder

using REST.NET.Data;
using REST.NET.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlServer<ApplicationDbContext>(connectionString);

/////////////////////
// Configure requests
var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenreEndpoints();

await app.MigrateDbAsync();

/////////////////////
// Start
app.Run();
