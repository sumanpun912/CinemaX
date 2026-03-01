using CinemaX.Domain.Common;

namespace CinemaX.Application.Auth;

public interface IAuthService
{
    Task<Result<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}