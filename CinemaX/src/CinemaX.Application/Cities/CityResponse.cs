using System;

namespace CinemaX.Application.Cities;

public record CityResponse(Guid Id, string Name, string TimeZoneId);