using REST.NET.Dtos;
using REST.NET.Entities;

namespace REST.NET.Mapping;

public static class GenreMapping
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new(genre.Id, genre.Name);
    } 
}