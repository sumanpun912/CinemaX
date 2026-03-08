using CinemaX.Domain.Common;

namespace CinemaX.Application.Movies;

public interface IMovieService
{
    Task<IReadOnlyList<MovieResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MovieResponse>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default);
    Task<MovieResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<MovieResponse>> CreateAsync(CreateMovieRequest request, CancellationToken cancellationToken = default);
    Task<Result<MovieResponse>> UpdateAsync(Guid id, UpdateMovieRequest request, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}