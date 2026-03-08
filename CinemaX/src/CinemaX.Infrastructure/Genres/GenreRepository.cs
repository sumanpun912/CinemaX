using CinemaX.Application.Data;
using CinemaX.Application.Genres;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;
using Dapper;

namespace CinemaX.Infrastructure.Genres;

internal sealed class GenreRepository(ISqlConnectionFactory _connectionFactory) : IGenreRepository
{
    public async Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var genres = await connection.QueryAsync<Genre>(
            "SELECT \"Id\", \"Name\" FROM genres ORDER BY \"Name\"");
        return genres.ToList();
    }

    public async Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Genre>(
            "SELECT \"Id\", \"Name\" FROM genres WHERE \"Id\" = @Id", new { Id = id });
    }

    public async Task<Result<Genre>> CreateAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var id = genre.Id != default ? genre.Id : Guid.NewGuid();
        await connection.ExecuteAsync(
            "INSERT INTO genres (\"Id\", \"Name\") VALUES (@Id, @Name)",
            new { Id = id, genre.Name });
        return Result<Genre>.Ok(new Genre { Id = id, Name = genre.Name });
    }

    public async Task<Result<Genre>> UpdateAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync(
            "UPDATE genres SET \"Name\" = @Name WHERE \"Id\" = @Id",
            new { genre.Id, genre.Name });
        if (rows == 0)
            return Result<Genre>.Fail(Error.NotFound("Genre not found."));
        return Result<Genre>.Ok(genre);
    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync(
            "DELETE FROM genres WHERE \"Id\" = @Id", new { Id = id });
        if (rows == 0)
            return Result<Unit>.Fail(Error.NotFound("Genre not found."));
        return Result<Unit>.Ok(default);
    }
}