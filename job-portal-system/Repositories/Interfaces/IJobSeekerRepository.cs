using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IJobSeekerRepository : IGenericRepository<JobSeeker>
    {
        Task AddJobSeekerMagedAsync(RegisterJobSeekerViewModel jobSeeker);
        Task<(List<DisplayJobSeekerForAdminViewModel>? model, int totalPages)> DiplayJobSeekerForAdminViewModel(int page = 1, string email = null!, int pageSize = 5);
        Task<JobSeeker?> GetJobSeekerByEmailAsync(string email);
        Task<JobSeeker?> GetJobSeekerById(string id);
        Task<JobSeeker?> GetJobSeekerProfileAsync(string userId);
        Task<JobSeeker?> GetJobSeekerWithApplicationsForJobsAsync(string email);
        Task<List<JobSeeker>?> GetAllJobSeekersAsync();
        Task EditJobSeekerProfile(EditJobSeekerViewModel model, JobSeeker jobSeeker);
    }
}
