using System;

namespace CinemaX.Application.Theaters;

public record TheaterResponse(Guid Id, Guid CityId, string Name);