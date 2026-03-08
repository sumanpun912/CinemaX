namespace CinemaX.Domain.Entities;

public sealed class Genre
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}