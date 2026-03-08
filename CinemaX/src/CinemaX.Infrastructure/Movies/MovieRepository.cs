using CinemaX.Application.Data;
using CinemaX.Application.Movies;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;
using Dapper;

namespace CinemaX.Infrastructure.Movies;

internal sealed class MovieRepository(ISqlConnectionFactory _connectionFactory) : IMovieRepository
{
    private const string SelectSql = """
        SELECT m."Id", m."Title", m."Description", m."PosterUrl", m."GenreId", m."DurationMins"
        FROM movies m
        """;

    public async Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var movies = await connection.QueryAsync<Movie>(SelectSql + " ORDER BY m.\"Title\"");
        return movies.ToList();
    }

    public async Task<IReadOnlyList<Movie>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var movies = await connection.QueryAsync<Movie>(
            SelectSql + " WHERE m.\"GenreId\" = @GenreId ORDER BY m.\"Title\"",
            new { GenreId = genreId });
        return movies.ToList();
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Movie>(
            SelectSql + " WHERE m.\"Id\" = @Id",
            new { Id = id });
    }

    public async Task<Result<Movie>> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var id = movie.Id != default ? movie.Id : Guid.NewGuid();
        await connection.ExecuteAsync(
            """
            INSERT INTO movies ("Id", "Title", "Description", "PosterUrl", "GenreId", "DurationMins")
            VALUES (@Id, @Title, @Description, @PosterUrl, @GenreId, @DurationMins)
            """,
            new { Id = id, movie.Title, movie.Description, movie.PosterUrl, movie.GenreId, movie.DurationMins });
        return Result<Movie>.Ok(new Movie
        {
            Id = id,
            Title = movie.Title,
            Description = movie.Description,
            PosterUrl = movie.PosterUrl,
            GenreId = movie.GenreId,
            DurationMins = movie.DurationMins
        });
    }

    public async Task<Result<Movie>> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync(
            """
            UPDATE movies
            SET "Title" = @Title, "Description" = @Description, "PosterUrl" = @PosterUrl,
                "GenreId" = @GenreId, "DurationMins" = @DurationMins
            WHERE "Id" = @Id
            """,
            new { movie.Id, movie.Title, movie.Description, movie.PosterUrl, movie.GenreId, movie.DurationMins });
        if (rows == 0)
            return Result<Movie>.Fail(Error.NotFound("Movie not found."));
        return Result<Movie>.Ok(movie);
    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync(
            "DELETE FROM movies WHERE \"Id\" = @Id", new { Id = id });
        if (rows == 0)
            return Result<Unit>.Fail(Error.NotFound("Movie not found."));
        return Result<Unit>.Ok(default);
    }
}