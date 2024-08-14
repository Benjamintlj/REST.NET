using System.ComponentModel.DataAnnotations;

namespace REST.NET.Dtos;

public record class CreateGameDto(
    [Required][StringLength(50)] string Name, 
    [Required][StringLength(20)] string Genre, 
    [Range(0, 100)] decimal Price, 
    DateOnly ReleaseDate
);