using System.Runtime.CompilerServices;
using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class JobSeekerRepository : GenericRepository<JobSeeker>, IJobSeekerRepository
    {
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;

        public JobSeekerRepository(ApplicationDbContext context, IWebHostEnvironment env, UserManager<User> userManager) : base(context)
        {
            _env = env;
            _userManager = userManager;
        }

        public async Task AddJobSeekerMagedAsync(RegisterJobSeekerViewModel model)
        {
            User user = new()
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "jobseeker");

                JobSeeker jobSeeker = new()
                {
                    FirstName = model.FirstName ?? "",
                    LastName = model.LastName ?? "",
                    UserId = user.Id
                };

                _context.JobSeekers.Add(jobSeeker);
                await _context.SaveChangesAsync();
            }

            else
            {
                foreach (var erorr in result.Errors)
                    Console.WriteLine($"Error occured while created new JobSeeekr by the admin: {erorr.Description}");
            }
        }

        public async Task<(List<DisplayJobSeekerForAdminViewModel>? model, int totalPages)> DiplayJobSeekerForAdminViewModel(int page = 1, string email = null!, int pageSize = 5)
        {
            if (email is null)
            {
                var jobSeekers = await _context.JobSeekers
                    .Include(x => x.User)
                    .ToListAsync();

                var totalCount = jobSeekers.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                List<DisplayJobSeekerForAdminViewModel> model = jobSeekers.Select(x => new DisplayJobSeekerForAdminViewModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.User.Email ?? "",
                    Education = x.Education,
                    ExperienceYears = x.ExperienceYears,
                    IsActive = x.User.IsActive,
                }).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return (model, totalPages);
            }
            else
            {
                var jobSeeker = await _context.JobSeekers
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.User.Email == email);

                var totalCount = await _context.JobSeekers.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                if (jobSeeker is null)
                {
                    return (null, totalPages);
                }

                DisplayJobSeekerForAdminViewModel model = new()
                {
                    Id = jobSeeker.Id,
                    UserId = jobSeeker.UserId,
                    FirstName = jobSeeker.FirstName,
                    LastName = jobSeeker.LastName,
                    Email = jobSeeker.User.Email ?? "",
                    Education = jobSeeker.Education,
                    ExperienceYears = jobSeeker.ExperienceYears,
                    IsActive = jobSeeker.User.IsActive,
                };

                List<DisplayJobSeekerForAdminViewModel> modelreturn = new();
                modelreturn.Add(model);

                return (modelreturn, totalPages);
            }
        }

        public async Task EditJobSeekerProfile(EditJobSeekerViewModel model, JobSeeker jobSeeker)
        {
            jobSeeker.FirstName = model.FirstName;
            jobSeeker.LastName = model.LastName;
            jobSeeker.Education = model.Education;
            jobSeeker.ExperienceYears = model.ExperienceYears;
            jobSeeker.Location = model.Location;
            jobSeeker.Skills = model.Skills;


            if (model.ImageFile is not null)
            {
                var oldImage = jobSeeker.ImagePath;
                var newImagePath = $"{Guid.NewGuid()}.{Path.GetExtension(model.ImageFile.FileName)}";
                var newImageFullPath = Path.Combine($"{_env.WebRootPath}/Images", newImagePath);

                using (var stream = File.Create(newImageFullPath))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                jobSeeker.ImagePath = newImagePath;

                if (oldImage is not null)
                    File.Delete(Path.Combine($"{_env.WebRootPath}/Images", oldImage));
            }

            if (model.CVFile is not null)
            {
                var oldCVPath = jobSeeker.CVPath;
                var newCVPath = $"{Guid.NewGuid()}{Path.GetExtension(model.CVFile.FileName)}";
                var newCVFullPath = Path.Combine($"{_env.WebRootPath}/cvs", newCVPath);

                using (var stream = File.Create(newCVFullPath))
                {
                    await model.CVFile.CopyToAsync(stream);
                }

                jobSeeker.CVPath = newCVPath;


                if (oldCVPath is not null)
                    File.Delete(Path.Combine($"{_env.WebRootPath}/cvs", oldCVPath));
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<JobSeeker>?> GetAllJobSeekersAsync()
        {
            return await _context.JobSeekers
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<JobSeeker?> GetJobSeekerByEmailAsync(string email)
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(js => js.User.Email == email);
        }

        public async Task<JobSeeker?> GetJobSeekerById(string id)
        {
            return _context.JobSeekers
                .Include(x => x.FavoriteJobs)
                .ThenInclude(x => x.Job)
                .ThenInclude(x => x.Category).FirstOrDefault(x => x.UserId == id);
        }

        public async Task<JobSeeker?> GetJobSeekerProfileAsync(string userId)
        {
            return await _context.JobSeekers.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public Task<JobSeeker?> GetJobSeekerWithApplicationsForJobsAsync(string email)
        {
            return _context.JobSeekers
                .Include(x => x.Applications)
                .ThenInclude(x => x.Job)
                .ThenInclude(x => x.Category).FirstOrDefaultAsync(x => x.User.Email == email);
        }
    }
}
