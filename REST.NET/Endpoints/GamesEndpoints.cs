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
        
        group.MapGet("/", (ApplicationDbContext dbContext) => dbContext.Games.Include(g => g.Genre).Select(g => g.ToGameSummaryDto()).AsNoTracking());

        group.MapGet("/{id}", (int id, ApplicationDbContext dbContext) =>
            {
                Game? game = dbContext.Games.Include(g => g.Genre).FirstOrDefault(g => g.Id == id);
                return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
            })
            .WithName(getGameEndpointName);

        group.MapPost("/", (CreateGameDto newGame, ApplicationDbContext dbContext) =>
        {
            var genre = dbContext.Genres.SingleOrDefault(g => g.Id == newGame.GenreId);
            if (genre == null)
            {
                return Results.NotFound("Genre not found.");
            }

            Game game = newGame.ToEntity();
            game.Genre = dbContext.Genres.Find(newGame.GenreId);
    
            dbContext.Games.Add(game);
            dbContext.SaveChanges();
            
            return Results.CreatedAtRoute(getGameEndpointName, new {id = game.Id}, game.ToGameDetailsDto());
        });

        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, ApplicationDbContext dbContext) =>
        {
            Game? game = dbContext.Games.FirstOrDefault(g => g.Id == id);
            if (game is null) return Results.NotFound();

            updatedGame.ToEntity();
            
            game.Name = updatedGame.Name;
            game.GenreId = updatedGame.GenreId;
            game.Genre = dbContext.Genres.Find(updatedGame.GenreId);
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;
            
            dbContext.Games.Update(game);
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id, ApplicationDbContext dbContext) =>
        {
            dbContext.Games.Where(game => game.Id == id).ExecuteDelete();

            return Results.NoContent();
        });

        return group;
    }
}