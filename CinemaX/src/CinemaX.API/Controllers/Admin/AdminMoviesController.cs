using CinemaX.API.Common;
using CinemaX.Application.Movies;
using CinemaX.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers.Admin;

[ApiController]
[Route("api/admin/movies")]
[Authorize(Roles = "Admin")]
public class AdminMoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IValidator<CreateMovieRequest> _createValidator;
    private readonly IValidator<UpdateMovieRequest> _updateValidator;

    public AdminMoviesController(
        IMovieService movieService,
        IValidator<CreateMovieRequest> createValidator,
        IValidator<UpdateMovieRequest> updateValidator)
    {
        _movieService = movieService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

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

    [HttpPost]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await _movieService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await _movieService.UpdateAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _movieService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return NoContent();
    }
}