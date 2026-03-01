namespace CinemaX.Application.Auth;

public record AuthResponse(string Id, string Email, string Token, List<string> Roles);