using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class JobSeekerRepository : GenericRepository<JobSeeker>, IJobSeekerRepository
    {
        private readonly IWebHostEnvironment _env;
        public JobSeekerRepository(ApplicationDbContext context, IWebHostEnvironment env) : base(context)
        {
            _env = env;
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
                var newCVPath = $"{Guid.NewGuid()}.{Path.GetExtension(model.CVFile.FileName)}";
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

        public async Task<JobSeeker?> GetByEmailAsync(string email)
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
    }
}
