using job_portal_system.Models.Domain;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<int> GetTotalJobSeekersAsync();
        Task<int> GetTotalEmployersAsync();
        Task<int> GetTotalJobsAsync();
        Task<List<int>> GetAllSalariesForAllJobs();
        Task<Dictionary<string, int>> GetJobsCountByCategoriesAsync();
        Task<int> GetPendingEmployersAsync();
        Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync();
        Task ApproveEmployerAsync(string employerId);
    }
}
