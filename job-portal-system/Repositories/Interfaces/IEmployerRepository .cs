using job_portal_system.Models.Domain;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IEmployerRepository : IGenericRepository<Employer>
    {
        Task<Employer?> GetByEmailAsync(string email);
    }
}
