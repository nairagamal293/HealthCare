// Data/DbInitializer.cs
using HeathCare.Models;
using HeathCare.Models.HeathCare.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace HealthCare.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            // Create database if not exists
            await context.Database.EnsureCreatedAsync();

            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create admin user
            var adminEmail = "admin@healthcare.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "System",
                    LastName = "Admin"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (!result.Succeeded)
                {
                    throw new Exception($"Admin user creation failed: {string.Join(", ", result.Errors)}");
                }

                // Assign admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}