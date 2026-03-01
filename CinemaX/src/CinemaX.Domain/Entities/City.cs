using System;

namespace CinemaX.Domain.Entities;

public sealed class City
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string TimeZoneId { get; set; }
}