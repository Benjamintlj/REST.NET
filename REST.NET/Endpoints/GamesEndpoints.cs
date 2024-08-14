using Microsoft.EntityFrameworkCore;
using REST.NET.Data;
using REST.NET.Dtos;
using REST.NET.Entities;
using REST.NET.Mapping;

namespace REST.NET.Endpoints;

public static class GamesEndpoints
{
    const string getGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        
        group.MapGet("/", async (ApplicationDbContext dbContext) => await dbContext.Games.Include(g => g.Genre).Select(g => g.ToGameSummaryDto()).AsNoTracking().ToListAsync());

        group.MapGet("/{id}", async (int id, ApplicationDbContext dbContext) =>
            {
                Game? game = await dbContext.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id);
                return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
            })
            .WithName(getGameEndpointName);

        group.MapPost("/", async (CreateGameDto newGame, ApplicationDbContext dbContext) =>
        {
            var genre = await dbContext.Genres.SingleOrDefaultAsync(g => g.Id == newGame.GenreId);
            if (genre == null)
            {
                return Results.NotFound("Genre not found.");
            }

            Game game = newGame.ToEntity();
            game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);
    
            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();
            
            return Results.CreatedAtRoute(getGameEndpointName, new {id = game.Id}, game.ToGameDetailsDto());
        });

        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, ApplicationDbContext dbContext) =>
        {
            Game? game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game is null) return Results.NotFound();

            updatedGame.ToEntity();
            
            game.Name = updatedGame.Name;
            game.GenreId = updatedGame.GenreId;
            game.Genre = dbContext.Genres.Find(updatedGame.GenreId);
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;
            
            dbContext.Games.Update(game);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, ApplicationDbContext dbContext) =>
        {
            await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}