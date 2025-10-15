using job_portal_system.Models.Domain;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalEmployersAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetPendingEmployersAsync();
        Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync();
        Task ApproveEmployerAsync(string employerId);
    }
}
