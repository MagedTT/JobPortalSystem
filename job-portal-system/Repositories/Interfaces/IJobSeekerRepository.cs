using job_portal_system.Models.Domain;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IJobSeekerRepository : IGenericRepository<JobSeeker>
    {
        Task<JobSeeker?> GetByEmailAsync(string email);
    }
}
