namespace CinemaX.Application.Movies;

public record UpdateMovieRequest(string Title, string? Description, string? PosterUrl, Guid GenreId, int DurationMins);