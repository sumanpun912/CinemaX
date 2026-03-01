using CinemaX.API.Common;
using CinemaX.Application.Cities;
using CinemaX.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers.Admin;

[ApiController]
[Route("api/admin/cities")]
[Authorize(Roles = "Admin")]
public class AdminCitiesController(ICityService cityService, 
    IValidator<CreateCityRequest> createValidator, 
    IValidator<UpdateCityRequest> updateValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cities = await cityService.GetAllAsync(cancellationToken);
        return Ok(cities);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var city = await cityService.GetByIdAsync(id, cancellationToken);
        return city is null ? NotFound() : Ok(city);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCityRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await createValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await cityService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await updateValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return ResultExtensions.ToActionResult(Result<object>.Fail(error));

        var result = await cityService.UpdateAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await cityService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return NoContent();
    }
}