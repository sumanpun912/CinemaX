namespace CinemaX.Application.Theaters;

public record CreateTheaterRequest(Guid CityId, string Name);