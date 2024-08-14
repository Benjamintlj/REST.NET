using REST.NET.Dtos;

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

        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
    
            games.Add(game);

            return Results.CreatedAtRoute(getGameEndpointName, new {id = game.Id}, game);
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