using Microsoft.EntityFrameworkCore;
using REST.NET.Data;
using REST.NET.Dtos;
using REST.NET.Mapping;

namespace REST.NET.Endpoints;

public static class GenreEndpoints
{
    public static RouteGroupBuilder MapGenreEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres");

        group.MapGet("/", async (ApplicationDbContext dbContext) =>
        {
            List<GenreDto> genres = await dbContext.Genres.Select(genre => genre.ToDto()).AsNoTracking().ToListAsync();

            return Results.Ok(genres);
        });

        return group;
    }
}