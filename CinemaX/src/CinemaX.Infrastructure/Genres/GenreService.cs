using CinemaX.Application.Genres;
using CinemaX.Domain.Common;
using CinemaX.Domain.Entities;

namespace CinemaX.Infrastructure.Genres;

internal sealed class GenreService(IGenreRepository _repository) : IGenreService
{
    public async Task<IReadOnlyList<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var genres = await _repository.GetAllAsync(cancellationToken);
        return genres.Select(Map).ToList();
    }

    public async Task<GenreResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var genre = await _repository.GetByIdAsync(id, cancellationToken);
        return genre is null ? null : Map(genre);
    }

    public async Task<Result<GenreResponse>> CreateAsync(CreateGenreRequest request, CancellationToken cancellationToken = default)
    {
        var genre = new Genre { Name = request.Name };
        var result = await _repository.CreateAsync(genre, cancellationToken);
        return result.IsSuccess ? Result<GenreResponse>.Ok(Map(result.Value!)) : Result<GenreResponse>.Fail(result.Error!);
    }

    public async Task<Result<GenreResponse>> UpdateAsync(Guid id, UpdateGenreRequest request, CancellationToken cancellationToken = default)
    {
        var genre = new Genre { Id = id, Name = request.Name };
        var result = await _repository.UpdateAsync(genre, cancellationToken);
        return result.IsSuccess ? Result<GenreResponse>.Ok(Map(result.Value!)) : Result<GenreResponse>.Fail(result.Error!);
    }

    public async Task<Result<Unit>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id, cancellationToken);

    private static GenreResponse Map(Genre genre) => new(genre.Id, genre.Name);
}