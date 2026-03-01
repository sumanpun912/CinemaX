using CinemaX.Application.Cities;
using CinemaX.Application.Theaters;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController(ICityService cityService, 
    ITheaterService theaterService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cities = await cityService.GetAllAsync(cancellationToken);
        return Ok(cities);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var city = await cityService.GetByIdAsync(id, cancellationToken);
        return city is null ? NotFound() : Ok(city);
    }

    [HttpGet("{id:guid}/theaters")]
    [ProducesResponseType(typeof(IReadOnlyList<TheaterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTheaters(Guid id, CancellationToken cancellationToken)
    {
        var city = await cityService.GetByIdAsync(id, cancellationToken);
        if (city is null)
            return NotFound();
        var theaters = await theaterService.GetByCityIdAsync(id, cancellationToken);
        return Ok(theaters);
    }
}