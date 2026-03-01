using System.Net;

namespace CinemaX.Domain.Common;

public record Error(string Code, string Message, HttpStatusCode? StatusCode = null)
{
  public static Error Validation(string message) => new("Validation.Failed", message, HttpStatusCode.BadRequest);
  public static Error NotFound(string message) => new("Entity.NotFound", message, HttpStatusCode.NotFound);
  public static Error Conflict(string message) => new("Entity.Conflict", message, HttpStatusCode.Conflict);
  public static Error Forbidden(string message) => new("Auth.Forbidden", message, HttpStatusCode.Forbidden);
  public static Error Unauthorized(string message) => new("Auth.Unauthorized", message, HttpStatusCode.Unauthorized);
};