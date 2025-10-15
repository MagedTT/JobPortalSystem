using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Services.Implementations
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly ApplicationDbContext _context;

        public JobSeekerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddJobSeekerAsync(JobSeeker jobSeeker)
        {
            await _context.JobSeekers.AddAsync(jobSeeker);
            await _context.SaveChangesAsync();
        }

        public async Task<JobSeeker?> GetByUserIdAsync(string userId)
        {
            return await _context.JobSeekers.FirstOrDefaultAsync(j => j.UserId == userId);
        }
    }
}
