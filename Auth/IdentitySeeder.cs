using graphql_proj_Csharp.Models;
using Microsoft.AspNetCore.Identity;

namespace graphql_proj_Csharp.Auth;

public static class IdentitySeeder
{
    public static async Task SeedIdentityAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue("Identity:SeedDefaults", true))
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in AuthRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = app.Configuration["Identity:DefaultAdmin:Email"];
        var adminPassword = app.Configuration["Identity:DefaultAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                FullName = "Default Admin",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed default admin: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AuthRoles.Admin))
        {
            await userManager.AddToRoleAsync(admin, AuthRoles.Admin);
        }
    }
}
