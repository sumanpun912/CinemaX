using CinemaX.Application.Genres;
using CinemaX.Application.Movies;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Infrastructure.Movies;

internal sealed class MovieService(IMovieRepository _repository, IGenreRepository _genreRepository) : IMovieService
{
    public async Task<IReadOnlyList<MovieResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var movies = await _repository.GetAllAsync(cancellationToken);
        return await MapManyAsync(movies, cancellationToken);
    }

    public async Task<IReadOnlyList<MovieResponse>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default)
    {
        var movies = await _repository.GetByGenreAsync(genreId, cancellationToken);
        return await MapManyAsync(movies, cancellationToken);
    }

    public async Task<MovieResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movie = await _repository.GetByIdAsync(id, cancellationToken);
        if (movie is null) return null;
        var genre = await _genreRepository.GetByIdAsync(movie.GenreId, cancellationToken);
        return Map(movie, genre?.Name ?? string.Empty);
    }

    public async Task<Result<MovieResponse>> CreateAsync(CreateMovieRequest request, CancellationToken cancellationToken = default)
    {
        var genre = await _genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        if (genre is null)
            return Result<MovieResponse>.Fail(Error.NotFound("Genre not found."));

        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            PosterUrl = request.PosterUrl,
            GenreId = request.GenreId,
            DurationMins = request.DurationMins
        };
        var result = await _repository.CreateAsync(movie, cancellationToken);
        return result.IsSuccess
            ? Result<MovieResponse>.Ok(Map(result.Value!, genre.Name))
            : Result<MovieResponse>.Fail(result.Error!);
    }

    public async Task<Result<MovieResponse>> UpdateAsync(Guid id, UpdateMovieRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Result<MovieResponse>.Fail(Error.NotFound("Movie not found."));

        var genre = await _genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        if (genre is null)
            return Result<MovieResponse>.Fail(Error.NotFound("Genre not found."));

        var movie = new Movie
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            PosterUrl = request.PosterUrl,
            GenreId = request.GenreId,
            DurationMins = request.DurationMins
        };
        var result = await _repository.UpdateAsync(movie, cancellationToken);
        return result.IsSuccess
            ? Result<MovieResponse>.Ok(Map(result.Value!, genre.Name))
            : Result<MovieResponse>.Fail(result.Error!);
    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id, cancellationToken);

    private async Task<IReadOnlyList<MovieResponse>> MapManyAsync(IReadOnlyList<Movie> movies, CancellationToken cancellationToken)
    {
        var allGenres = await _genreRepository.GetAllAsync(cancellationToken);
        var genreMap = allGenres.ToDictionary(g => g.Id, g => g.Name);
        return movies.Select(m => Map(m, genreMap.GetValueOrDefault(m.GenreId, string.Empty))).ToList();
    }

    private static MovieResponse Map(Movie movie, string genreName) =>
        new(movie.Id, movie.Title, movie.Description, movie.PosterUrl, movie.GenreId, genreName, movie.DurationMins);

}