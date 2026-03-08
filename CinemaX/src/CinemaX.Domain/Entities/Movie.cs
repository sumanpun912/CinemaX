namespace CinemaX.Domain.Entities;

public sealed class Movie
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? PosterUrl { get; set; }
    public Guid GenreId { get; set; }
    public int DurationMins { get; set; }
}