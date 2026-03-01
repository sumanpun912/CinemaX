using CinemaX.Application.Cities;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Infrastructure.Cities;

public sealed class CityService(ICityRepository _repository) : ICityService
{
    public async Task<IReadOnlyList<CityResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cities = await _repository.GetAllAsync(cancellationToken);
        return cities.Select(Map).ToList();
    }

    public async Task<CityResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var city = await _repository.GetByIdAsync(id, cancellationToken);
        return city is null ? null : Map(city);
    }

    public async Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default)
    {
        var city = new City { Name = request.Name, TimeZoneId = request.TimeZoneId };
        var result = await _repository.CreateAsync(city, cancellationToken);
        return result.IsSuccess ? Result<CityResponse>.Ok(Map(result.Value!)) : Result<CityResponse>.Fail(result.Error!);

    }

    public async Task<Result<CityResponse>> UpdateAsync(Guid id, UpdateCityRequest request, CancellationToken cancellationToken = default)
    {
        var city = new City { Id = id, Name = request.Name, TimeZoneId = request.TimeZoneId };
        var result = await _repository.UpdateAsync(city, cancellationToken);
        return result.IsSuccess ? Result<CityResponse>.Ok(Map(result.Value!)) : Result<CityResponse>.Fail(result.Error!);

    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id, cancellationToken);

    private static CityResponse Map(City city) => new(city.Id, city.Name, city.TimeZoneId);
}