using System.Text;
using CinemaX.Application.Auth;
using CinemaX.Application.Cities;
using CinemaX.Application.Data;
using CinemaX.Application.Genres;
using CinemaX.Application.Movies;
using CinemaX.Application.Theaters;
using CinemaX.Infrastructure.Auth;
using CinemaX.Infrastructure.Cities;
using CinemaX.Infrastructure.Data;
using CinemaX.Infrastructure.Genres;
using CinemaX.Infrastructure.Identity;
using CinemaX.Infrastructure.Movies;
using CinemaX.Infrastructure.Theaters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CinemaX.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ??
                               throw new InvalidOperationException("Connection string 'Default' not configured.");

        // EF Core: models, DbContext, migrations, Identity
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Dapper: data operations (queries, commands)
        services.AddSingleton<ISqlConnectionFactory, NpgsqlConnectionFactory>();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "CinemaX",
                    ValidAudience = configuration["Jwt:Audience"] ?? "CinemaX",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured")))
                };
            });

        services.AddScoped<IAuthService, AuthService>();
        
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ITheaterRepository, TheaterRepository>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ITheaterService, TheaterService>();
        
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}