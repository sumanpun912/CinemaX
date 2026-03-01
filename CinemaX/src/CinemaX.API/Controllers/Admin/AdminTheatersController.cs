using CinemaX.API.Common;
using CinemaX.Application.Theaters;
using CinemaX.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers.Admin;

[ApiController]
[Route("api/admin/theaters")]
[Authorize(Roles = "Admin")]
public class AdminTheatersController(ITheaterService theaterService,
    IValidator<CreateTheaterRequest> createValidator,
    IValidator<UpdateTheaterRequest> updateValidator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TheaterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var theater = await theaterService.GetByIdAsync(id, cancellationToken);
        return theater is null ? NotFound() : Ok(theater);
    }

    [HttpGet("by-city/{cityId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<TheaterResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCityId(Guid cityId, CancellationToken cancellationToken)
    {
        var theaters = await theaterService.GetByCityIdAsync(cityId, cancellationToken);
        return Ok(theaters);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TheaterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateTheaterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await createValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await theaterService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TheaterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTheaterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await updateValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await theaterService.UpdateAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await theaterService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return NoContent();
    }
}