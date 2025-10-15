using System.Net.WebSockets;
using System.Threading.Tasks;
using job_portal_system.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;

namespace job_portal_system.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            var users = new[]
            {
                new { UserName = "admin123", Email = "admin123@gmail.com", Password = "Password123!", Role = "admin" },
                new { UserName = "user2", Email = "user2@gmail.com", Password = "Password123!", Role = "employer" },
                new { UserName = "user3", Email = "user3@gmail.com", Password = "Password123!", Role = "employer" },
                new { UserName = "user4", Email = "user4@gmail.com", Password = "Password123!", Role = "employer" },
                new { UserName = "user5", Email = "user5@gmail.com", Password = "Password123!", Role = "employer" },
                new { UserName = "user6", Email = "user6@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user7", Email = "user7@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user8", Email = "user8@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user9", Email = "user9@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user10", Email = "user10@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user11", Email = "user11@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user12", Email = "user12@gmail.com", Password = "Password123!", Role = "jobseeker" },
                new { UserName = "user13", Email = "user13@gmail.com", Password = "Password123!", Role = "jobseeker" },
            };

            var roles = new[] { "admin", "jobseeker", "employer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var companiesNames = new string[] { "Google", "Facebook", "Microsoft", "Apple" };
            var companiesDescriptions = new string[]
            {
                "Google Company Description",
                "Facebook Company Description",
                "Microsoft Company Description",
                "Apple Company Description"
            };

            var companiesLocations = new string[]
            {
                "Mansoura",
                "Tanta",
                "Alexandria",
                "Cairo"
            };

            var ids = new string[]
            {
                "af2fc054-b700-47a6-988e-cd26df4eb2f5",
                "c0f9df0d-037c-4d12-a4e0-0c5244725292",
                "530c97c3-7775-49d3-8ce1-097742f14eef",
                "08e004f3-f6f5-41d7-be54-e43484435735",
                "eb28d096-bf43-4b25-9647-f096f0f71f8d",
                "29a76f0c-1a47-4b32-8bc7-82bf517f1463",
                "e57efcf8-0344-4921-9389-9cf53c126582",
                "fa3940bb-13a0-4222-884b-fd12332bb323",
                "c17d861a-a1b2-4e8e-af96-1a53520be18f",
                "f8725844-7178-4da0-9109-24fbf842350c",
                "a316f2b6-0d2d-4b67-afba-a3caf88b1918",
                "c8226405-c22c-4cf7-8c3b-90da02366e72"
            };

            var educations = new string[]
            {
                "Bachelors Degree",
                "PhD",
                "Masters Degree"
            };

            var random = new Random();

            foreach (var user in users)
            {
                User userToSeed;
                if (await userManager.FindByEmailAsync(user.Email!) == null)
                {
                    userToSeed = new User
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                    };

                    var result = await userManager.CreateAsync(userToSeed, user.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(userToSeed, user.Role);

                        if (user.Role == "employer")
                        {
                            var employerExists = await context.Employers.FirstOrDefaultAsync(x => x.Id == userToSeed.Id);
                            if (employerExists is null)
                            {
                                var employer = new Employer
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    UserId = userToSeed.Id,
                                    CompanyName = companiesNames[random.Next(companiesNames.Length)],
                                    Description = companiesDescriptions[random.Next(companiesDescriptions.Length)],
                                    Location = companiesLocations[random.Next(companiesLocations.Length)],
                                    IsApproved = true
                                };

                                context.Employers.Add(employer);
                                context.SaveChanges();
                            }
                        }

                        else if (user.Role == "jobseeker")
                        {
                            var jobSeekerExists = await context.JobSeekers.FirstOrDefaultAsync(x => x.Id == userToSeed.Id);

                            if (jobSeekerExists is null)
                            {
                                var jobSeeker = new JobSeeker
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    UserId = userToSeed.Id,
                                    FirstName = user.UserName,
                                    LastName = user.UserName,
                                    Education = educations[random.Next(educations.Length)],
                                    ExperienceYears = random.Next(10),
                                    Location = companiesLocations[random.Next(companiesLocations.Length)]
                                };

                                context.JobSeekers.Add(jobSeeker);
                                context.SaveChanges();
                            }
                        }
                    }

                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"==========> Error Occured while seeding the users: {error.Description}");
                        }
                    }
                }
            }

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

            var categoriesIds = context.Categories.Select(x => x.Id).ToList();

            var employersIds = context.Employers.Select(x => x.Id).ToList();
            if (!context.Jobs.Any())
            {
                for (var i = 0; i < 10; ++i)
                {
                    var job = new Job
                    {
                        EmployerId = employersIds[random.Next(employersIds.Count)],
                        Title = "This is a title for the job",
                        Description = "This is a description for the job",
                        YearsOfExperience = random.Next(10),
                        Salary = random.Next(10000, 20000),
                        Location = companiesLocations[random.Next(companiesLocations.Length)],
                        CategoryId = categoriesIds[random.Next(categoriesIds.Count)],
                        JobType = JobType.FullTime,
                        PostedAt = DateTime.Now,
                        IsActive = true
                    };

                    context.Jobs.Add(job);
                }

                context.SaveChanges();
            }
        }
    }
}
