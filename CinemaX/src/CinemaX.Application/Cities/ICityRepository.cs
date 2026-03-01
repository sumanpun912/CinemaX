using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Application.Cities;

public interface ICityRepository
{
    Task<IReadOnlyList<City>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<City?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<City>> CreateAsync(City city, CancellationToken cancellationToken = default);
    Task<Result<City>> UpdateAsync(City city, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}