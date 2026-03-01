using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Application.Theaters;

public interface ITheaterRepository
{
    Task<IReadOnlyList<Theater>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default);
    Task<Theater?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Theater>> CreateAsync(Theater theater, CancellationToken cancellationToken = default);
    Task<Result<Theater>> UpdateAsync(Theater theater, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}