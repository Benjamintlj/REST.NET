using System.ComponentModel.DataAnnotations;

namespace REST.NET.Dtos;

public record class UpdateGameDto(
    [Required][StringLength(50)] string Name, 
    [Required]int GenreId, 
    [Range(0, 100)] decimal Price, 
    DateOnly ReleaseDate
);