using CinemaX.Application.Genres;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController(IGenreService _genreService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GenreResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var genres = await _genreService.GetAllAsync(cancellationToken);
        return Ok(genres);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genreService.GetByIdAsync(id, cancellationToken);
        return genre is null ? NotFound() : Ok(genre);
    }
}