using CinemaX.Application.Data;
using CinemaX.Application.Theaters;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;
using Dapper;

namespace CinemaX.Infrastructure.Theaters;

internal sealed class TheaterRepository(ISqlConnectionFactory _connectionFactory) : ITheaterRepository
{
    public async Task<IReadOnlyList<Theater>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var theaters = await connection.QueryAsync<Theater>(
            "SELECT \"Id\", \"CityId\", \"Name\" FROM theaters WHERE \"CityId\" = @CityId ORDER BY \"Name\"",
            new { CityId = cityId });
        return theaters.ToList();
    }

    public async Task<Theater?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Theater>(
            "SELECT \"Id\", \"CityId\", \"Name\" FROM theaters WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<Result<Theater>> CreateAsync(Theater theater, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var id = theater.Id != default ? theater.Id : Guid.NewGuid();
        await connection.ExecuteScalarAsync<int>(
            "INSERT INTO theaters (\"CityId\", \"Name\") VALUES (@CityId, @Name)",
            new { theater.CityId, theater.Name });
        return Result<Theater>.Ok(new Theater { Id = id, CityId = theater.CityId, Name = theater.Name });

    }

    public async Task<Result<Theater>> UpdateAsync(Theater theater, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync(
            "UPDATE theaters SET \"Name\" = @Name WHERE \"Id\" = @Id",
            new { theater.Id, theater.Name });
        if (rows == 0)
            return Result<Theater>.Fail(Error.NotFound("Theater not found."));
        return Result<Theater>.Ok(theater);

    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync("DELETE FROM theaters WHERE \"Id\" = @Id", new { Id = id });
        if (rows == 0)
            return Result<Unit>.Fail(Error.NotFound("Theater not found."));
        return Result<Unit>.Ok(default);

    }
}