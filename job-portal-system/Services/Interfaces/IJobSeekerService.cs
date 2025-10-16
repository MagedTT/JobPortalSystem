using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Services.Interfaces
{
    public interface IJobSeekerService
    {
        Task AddJobSeekerAsync(JobSeeker jobSeeker);
        Task<JobSeeker?> GetJobSeekerByIdAsync(string userId);
        Task<JobSeeker?> GetJobSeekerProfileAsync(string userId);
        Task EditJobSeekerProfileAsync(EditJobSeekerViewModel model, JobSeeker jobSeeker);
    }
}
