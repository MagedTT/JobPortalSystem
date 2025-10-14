using System.Net.WebSockets;
using System.Threading.Tasks;
using job_portal_system.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Software Development" },
                    new Category { Name = "Game Development" },
                    new Category { Name = "Finance" },
                    new Category { Name = "HR" },
                    new Category { Name = "AI" },
                    new Category { Name = "Marketing" },
                    new Category { Name = "IT" },
                    new Category { Name = "Data Science" },
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            var adminEmail = "what@gmail.com";
            var adminPassword = "adminPasswordlol123*";

            if (!await roleManager.RoleExistsAsync("admin"))
                await roleManager.CreateAsync(new IdentityRole("admin"));

            if (!await roleManager.RoleExistsAsync("employer"))
                await roleManager.CreateAsync(new IdentityRole("employer"));

            if (!await roleManager.RoleExistsAsync("jobseeker"))
                await roleManager.CreateAsync(new IdentityRole("jobseeker"));

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin is null)
            {
                admin = new User
                {
                    UserName = "admin123",
                    Email = adminEmail,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"==========> Error when seeding the admin user: {error.Description}");
                    }
                }
            }
        }
    }
}
