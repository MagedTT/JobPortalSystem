using job_portal_system.Data;
using job_portal_system.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Add Roles
            string[] roles = new[] { "Admin", "Employer", "JobSeeker" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Admin user
            string adminEmail = "admin@jobverse.com";
            string adminPassword = "Admin123!";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 3. Employers
            var employers = new List<IdentityUser>();
            for (int i = 1; i <= 10; i++)
            {
                string email = $"employer{i}@jobverse.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "Employer123!");
                    await userManager.AddToRoleAsync(user, "Employer");
                    employers.Add(user);

                    // Add to Employers table
                    context.Employers.Add(new Employer
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        CompanyName = $"Company {i}",
                        Description = $"Description for Company {i}",
                        Location = $"City {i}",
                        IsApproved = true
                    });
                }
            }

            // 4. JobSeekers
            var jobSeekers = new List<IdentityUser>();
            for (int i = 1; i <= 30; i++)
            {
                string email = $"jobseeker{i}@jobverse.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "JobSeeker123!");
                    await userManager.AddToRoleAsync(user, "JobSeeker");
                    jobSeekers.Add(user);

                    // Add to JobSeekers table
                    context.JobSeekers.Add(new JobSeeker
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        FirstName = $"First{i}",
                        LastName = $"Last{i}",
                        Education = "Bachelor",
                        ExperienceYears = i % 10,
                        Location = $"City {i}",
                        Skills = "C#, .NET, SQL"
                    });
                }
            }

            await context.SaveChangesAsync();

            // 5. Jobs for each employer (1-3 jobs per employer)
            var random = new Random();
            var categories = await context.Categories.ToListAsync();
            if (!categories.Any())
            {
                // Add some categories if empty
                categories = new List<Category>
                {
                    new Category { Name = "IT" },
                    new Category { Name = "Marketing" },
                    new Category { Name = "Finance" },
                    new Category { Name = "HR" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            foreach (var employer in context.Employers)
            {
                int jobsCount = random.Next(1, 4);
                for (int j = 1; j <= jobsCount; j++)
                {
                    var category = categories[random.Next(categories.Count)];
                    context.Jobs.Add(new Job
                    {
                        EmployerId = employer.Id,
                        Title = $"Job {j} at {employer.CompanyName}",
                        Description = $"Description for Job {j} at {employer.CompanyName}",
                        YearsOfExperience = random.Next(0, 5),
                        Salary = random.Next(5000, 20000),
                        Location = employer.Location,
                        CategoryId = category.Id,
                        JobType = (JobType)random.Next(0, 2),
                        PostedAt = DateTime.Now,
                        IsActive = true
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
