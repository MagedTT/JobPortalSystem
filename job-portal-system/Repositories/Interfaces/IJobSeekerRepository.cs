using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IJobSeekerRepository : IGenericRepository<JobSeeker>
    {
        Task<JobSeeker?> GetByEmailAsync(string email);
        Task<JobSeeker?> GetJobSeekerById(string id);
        Task<JobSeeker?> GetJobSeekerProfileAsync(string userId);
        Task EditJobSeekerProfile(EditJobSeekerViewModel model, JobSeeker jobSeeker);
    }
}
