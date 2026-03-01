using CinemaX.Application.Cities;
using CinemaX.Application.Theaters;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Infrastructure.Theaters;

internal sealed class TheaterService(ITheaterRepository _repository, ICityRepository _cityRepository) : ITheaterService
{
    public async Task<IReadOnlyList<TheaterResponse>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        var theaters = await _repository.GetByCityIdAsync(cityId, cancellationToken);
        return theaters.Select(Map).ToList();
    }

    public async Task<TheaterResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var theater = await _repository.GetByIdAsync(id, cancellationToken);
        return theater is null ? null : Map(theater);
    }

    public async Task<Result<TheaterResponse>> CreateAsync(CreateTheaterRequest request, CancellationToken cancellationToken = default)
    {
        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
            return Result<TheaterResponse>.Fail(Error.NotFound("City not found."));
        var theater = new Theater { CityId = request.CityId, Name = request.Name };
        var result = await _repository.CreateAsync(theater, cancellationToken);
        return result.IsSuccess ? Result<TheaterResponse>.Ok(Map(result.Value!)) : Result<TheaterResponse>.Fail(result.Error!);

    }

    public async Task<Result<TheaterResponse>> UpdateAsync(Guid id, UpdateTheaterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Result<TheaterResponse>.Fail(Error.NotFound("Theater not found."));
        var theater = new Theater { Id = id, CityId = existing.CityId, Name = request.Name };
        var result = await _repository.UpdateAsync(theater, cancellationToken);
        return result.IsSuccess ? Result<TheaterResponse>.Ok(Map(result.Value!)) : Result<TheaterResponse>.Fail(result.Error!);

    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
         => await _repository.DeleteAsync(id, cancellationToken);
    
    private static TheaterResponse Map(Theater theater) => new(theater.Id, theater.CityId, theater.Name);

}