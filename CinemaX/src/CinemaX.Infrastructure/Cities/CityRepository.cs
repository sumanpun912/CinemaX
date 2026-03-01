using CinemaX.Application.Cities;
using CinemaX.Application.Data;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;
using Dapper;

namespace CinemaX.Infrastructure.Cities;

public sealed class CityRepository(ISqlConnectionFactory connectionFactory) : ICityRepository
{
    public async Task<IReadOnlyList<City>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var cities = await connection.QueryAsync<City>(
            @"SELECT ""Id"", ""Name"", ""TimeZoneId"" FROM cities ORDER BY ""Name""");
        return cities.ToList();
    }

    public async Task<City?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<City>(
            @"SELECT ""Id"", ""Name"", ""TimeZoneId"" FROM cities WHERE ""Id"" = @Id", new { Id = id });
    }

    public async Task<Result<City>> CreateAsync(City city, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        var id = city.Id != default ? city.Id : Guid.NewGuid();
        await connection.ExecuteScalarAsync<int>(
            "INSERT INTO cities (\"Name\", \"TimeZoneId\") VALUES (@Name, @TimeZoneId)",
            new { Id = id, city.Name, city.TimeZoneId });
        return Result<City>.Ok(new City { Id = id, Name = city.Name, TimeZoneId = city.TimeZoneId });

    }

    public async Task<Result<City>> UpdateAsync(City city, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync(
            "UPDATE cities SET \"Name\" = @Name, \"TimeZoneId\" = @TimeZoneId WHERE \"Id\" = @Id",
            new { city.Id, city.Name, city.TimeZoneId });
        if (rows == 0)
            return Result<City>.Fail(Error.NotFound("City not found."));
        return Result<City>.Ok(city);
    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        var rows = await connection.ExecuteAsync("DELETE FROM cities WHERE \"Id\" = @Id", new { Id = id });
        if (rows == 0)
            return Result<Unit>.Fail(Error.NotFound("City not found."));
        return Result<Unit>.Ok(default);
    }
}