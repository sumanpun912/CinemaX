using CinemaX.API.Common;
using CinemaX.Application.Genres;
using CinemaX.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers.Admin;

[ApiController]
[Route("api/admin/genres")]
[Authorize(Roles = "Admin")]
public class AdminGenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly IValidator<CreateGenreRequest> _createValidator;
    private readonly IValidator<UpdateGenreRequest> _updateValidator;

    public AdminGenresController(
        IGenreService genreService,
        IValidator<CreateGenreRequest> createValidator,
        IValidator<UpdateGenreRequest> updateValidator)
    {
        _genreService = genreService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

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

    [HttpPost]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGenreRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await _genreService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGenreRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await _genreService.UpdateAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _genreService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return NoContent();
    }
}