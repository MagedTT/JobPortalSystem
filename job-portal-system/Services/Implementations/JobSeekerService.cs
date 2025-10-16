using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Repositories.Interfaces;
using job_portal_system.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Services.Implementations
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly IJobSeekerRepository _jobSeekerRepository;
        private readonly ApplicationDbContext _context;

        public JobSeekerService(
            ApplicationDbContext context,
            IJobSeekerRepository jobSeekerRepository)
        {
            _context = context;
            _jobSeekerRepository = jobSeekerRepository;
        }

        public async Task AddJobSeekerAsync(JobSeeker jobSeeker)
        {
            await _context.JobSeekers.AddAsync(jobSeeker);
            await _context.SaveChangesAsync();
        }

        public async Task EditJobSeekerProfileAsync(EditJobSeekerViewModel model, JobSeeker jobSeeker)
        {
            await _jobSeekerRepository.EditJobSeekerProfile(model, jobSeeker);
        }

        public async Task<JobSeeker?> GetJobSeekerByIdAsync(string userId)
        {
            return await _jobSeekerRepository.GetJobSeekerById(userId);
        }

        public async Task<JobSeeker?> GetJobSeekerProfileAsync(string userId)
        {
            return await _jobSeekerRepository.GetJobSeekerProfileAsync(userId);
        }
    }
}
