using job_portal_system.Models.Domain;

namespace job_portal_system.Services.Interfaces
{
    public interface IJobSeekerService
    {
        Task AddJobSeekerAsync(JobSeeker jobSeeker);
        Task<JobSeeker?> GetByUserIdAsync(string userId);
    }
}
