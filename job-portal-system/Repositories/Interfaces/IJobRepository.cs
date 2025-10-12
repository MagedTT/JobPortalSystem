using job_portal_system.Models.Domain;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<IEnumerable<Job>> GetRecentJobsAsync(int count);
    }
}
