using CinemaX.API.Common;
using CinemaX.Application.Auth;
using CinemaX.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await registerValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return Result<object>.Fail(error).ToActionResult();

        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await loginValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.ToValidationError() is { } error)
            return Result<object>.Fail(error).ToActionResult();
        
        var result = await authService.LoginAsync(request, cancellationToken);
        return result.ToActionResult();
    }
}