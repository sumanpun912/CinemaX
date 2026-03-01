namespace CinemaX.Application.Auth;

public record UserResponse(string Id, string Email, List<string> Roles);
