using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Services.Interfaces
{
    public interface IJobSeekerService
    {
        Task AddJobSeekerAsync(JobSeeker jobSeeker);
        Task<JobSeeker?> GetJobSeekerByIdAsync(string userId);
        Task<JobSeeker?> GetJobSeekerProfileAsync(string userId);
        Task<JobSeeker?> GetJobSeekerWithApplicationsForJobsAsync(string email);
        Task<List<JobSeeker>?> GetAllJobSeekersAsync();
        Task<JobSeeker?> GetJobSeekerByEmailAsync(string email);
        Task<(List<DisplayJobSeekerForAdminViewModel>? model, int totalPages)> DiplayJobSeekerForAdminViewModel(int page = 1, string email = null!, int pageSize = 5);
        Task AddJobSeekerMagedAsync(RegisterJobSeekerViewModel jobSeeker);
        Task EditJobSeekerProfileAsync(EditJobSeekerViewModel model, JobSeeker jobSeeker);
    }
}
