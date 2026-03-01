using CinemaX.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CinemaX.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        await db.Database.MigrateAsync();

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new ApplicationRole { Name = adminRole, NormalizedName = "ADMIN" });

        const string userRole = "User";
        if (!await roleManager.RoleExistsAsync(userRole))
            await roleManager.CreateAsync(new ApplicationRole { Name = userRole, NormalizedName = "USER" });

        const string adminEmail = "admin@cinemax.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}