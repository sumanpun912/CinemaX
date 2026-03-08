using CinemaX.Domain.Common;

namespace CinemaX.Application.Genres;

public interface IGenreService
{
    Task<IReadOnlyList<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GenreResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GenreResponse>> CreateAsync(CreateGenreRequest request, CancellationToken cancellationToken = default);
    Task<Result<GenreResponse>> UpdateAsync(Guid id, UpdateGenreRequest request, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}