using System;

namespace CinemaX.Domain.Entities;

public sealed class Theater
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public required string Name { get; set; }
}