using CinemaX.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaX.Infrastructure.Identity;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<Theater> Theaters => Set<Theater>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<City>(entity =>
        {
            entity.ToTable("cities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.TimeZoneId).HasMaxLength(100).IsRequired();
        });
        builder.Entity<Theater>(entity =>
        {
            entity.ToTable("theaters");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.HasOne<City>()
                .WithMany()
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
