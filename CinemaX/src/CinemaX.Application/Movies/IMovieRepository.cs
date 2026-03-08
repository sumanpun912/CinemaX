using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Application.Movies;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Movie>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default);
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Movie>> CreateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<Result<Movie>> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}