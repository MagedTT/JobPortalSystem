using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IEmployerRepository : IGenericRepository<Employer>
    {
        Task<Employer?> GetByEmailAsync(string email);
        Task<Employer?> GetEmployerProfileAsync(string userId);
        Task EditEmployerProfile(EditEmployerViewModel model, Employer employer);
    }
}
