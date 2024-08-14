using REST.NET.Data;
using REST.NET.Dtos;
using REST.NET.Entities;

namespace REST.NET.Endpoints;

public static class GamesEndpoints
{
    const string getGameEndpointName = "GetGame";
    
    private static readonly  List<GameDto> games =
    [
        new (1, "MC", "fun", 11.99M, new DateOnly(1982, 4, 15)),
        new (2, "COD", "bad", 14.99M, new DateOnly(1992, 3, 15)),
        new (3, "BF1", "fun", 10.99M, new DateOnly(2001, 1, 15)),
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        
        group.MapGet("/", () => games);

        group.MapGet("/{id}", (int id) =>
            {
                GameDto? game = games.Find(g => g.Id == id);
                return game is null ? Results.NotFound() : Results.Ok(game);
            })
            .WithName(getGameEndpointName);

        group.MapPost("/", (CreateGameDto newGame, ApplicationDbContext dbContext) =>
        {
            var genre = dbContext.Genres.SingleOrDefault(g => g.Id == newGame.GenreId);
            if (genre == null)
            {
                return Results.NotFound("Genre not found.");
            }

            Game game = new()
            {
                Name = newGame.Name,
                Genre = genre,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };
    
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameDto gameDto = new(
                game.Id,
                game.Name,
                game.Genre.Name,
                game.Price,
                game.ReleaseDate
            );

            return Results.CreatedAtRoute(getGameEndpointName, new {id = game.Id}, gameDto);
        });

        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(g => g.Id == id);

            if (index == -1) return Results.NotFound();
    
            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });

        return group;
    }
}