using job_portal_system.Models.Domain;

namespace job_portal_system.Services.Interfaces
{
    public interface IAdminService
    {
        Task<int> GetTotalJobSeekersAsync();
        Task<int> GetTotalEmployersAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetPendingEmployersAsync();
        Task<List<int>> GetAllSalariesForAllJobs();
        Task<Dictionary<string, int>> GetJobsCountByCategoriesAsync();
        Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync();
        Task ApproveEmployerAsync(string employerId);
    }
}
