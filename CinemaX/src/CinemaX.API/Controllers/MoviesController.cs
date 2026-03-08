using CinemaX.Application.Movies;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService _movieService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MovieResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var movies = await _movieService.GetAllAsync(cancellationToken);
        return Ok(movies);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var movie = await _movieService.GetByIdAsync(id, cancellationToken);
        return movie is null ? NotFound() : Ok(movie);
    }

    [HttpGet("by-genre/{genreId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<MovieResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGenre(Guid genreId, CancellationToken cancellationToken)
    {
        var movies = await _movieService.GetByGenreAsync(genreId, cancellationToken);
        return Ok(movies);
    }
}