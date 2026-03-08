namespace CinemaX.Application.Movies;

public record MovieResponse(Guid Id, string Title, string? Description, string? PosterUrl, Guid GenreId, string GenreName, int DurationMins);