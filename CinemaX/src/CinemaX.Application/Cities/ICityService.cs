using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CinemaX.Domain.Common;

namespace CinemaX.Application.Cities;

public interface ICityService
{
    Task<IReadOnlyList<CityResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CityResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> UpdateAsync(Guid id, UpdateCityRequest request, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}