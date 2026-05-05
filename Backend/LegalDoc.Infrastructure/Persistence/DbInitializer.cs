using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LegalDoc.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        
        string[] roles = { "Admin", "Lawyer", "Viewer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var admin = new IdentityUser 
            { 
                UserName = "admin", 
                EmailConfirmed = true 
            };
            
            var result = await userManager.CreateAsync(admin, "admin"); 
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                Console.WriteLine("--> Admin user created successfully.");
            }
            else
            {
                // AFIȘARE ERORI: Foarte util pentru debug
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"--> ERROR seeding admin: {errors}");
            }
        }
    }
}