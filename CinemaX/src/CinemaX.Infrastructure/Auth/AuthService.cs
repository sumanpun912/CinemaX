using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CinemaX.Application.Auth;
using CinemaX.Domain.Common;
using CinemaX.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CinemaX.Infrastructure.Auth;

public sealed class AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser()
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var conflict = result.Errors.FirstOrDefault(
                e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName");
            if (conflict != null)
                return Result<UserResponse>.Fail(Error.Conflict("A user with the same email address already exists."));
            
            var msg = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<UserResponse>.Fail(Error.Validation(msg));
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        return Result<UserResponse>.Ok(new UserResponse(user.Id, user.UserName, roles.ToList()));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result<AuthResponse>.Fail(Error.Unauthorized("Invalid credentials"));

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid)
            return Result<AuthResponse>.Fail(Error.Unauthorized("Invalid credentials"));

        var token = await GenerateJwtAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        
        return Result<AuthResponse>.Ok(new AuthResponse(user.Id, user.Email!, token, roles.ToList()));
    }
    
    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issuer = _configuration["Jwt:Issuer"] ?? "PlanLearn";
        var audience = _configuration["Jwt:Audience"] ?? "PlanLearn";

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}