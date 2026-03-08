using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Application.Genres;

public interface IGenreRepository
{
    Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Genre>> CreateAsync(Genre genre, CancellationToken cancellationToken = default);
    Task<Result<Genre>> UpdateAsync(Genre genre, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}