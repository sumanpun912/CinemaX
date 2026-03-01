using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CinemaX.Domain.Common;

namespace CinemaX.Application.Theaters;

public interface ITheaterService
{
    Task<IReadOnlyList<TheaterResponse>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default);
    Task<TheaterResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TheaterResponse>> CreateAsync(CreateTheaterRequest request, CancellationToken cancellationToken = default);
    Task<Result<TheaterResponse>> UpdateAsync(Guid id, UpdateTheaterRequest request, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}