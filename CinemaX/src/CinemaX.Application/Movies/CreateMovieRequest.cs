namespace CinemaX.Application.Movies;

public record CreateMovieRequest(string Title, string? Description, string? PosterUrl, Guid GenreId, int DurationMins);