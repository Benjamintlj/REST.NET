/////////////////////
// Configure builder

using REST.NET.Dtos;
using REST.NET.Endpoints;

var builder = WebApplication.CreateBuilder(args);

/////////////////////
// Configure requests
var app = builder.Build();

app.MapGamesEndpoints();

/////////////////////
// Start
app.Run();
