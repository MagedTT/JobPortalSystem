using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Services.Interfaces
{
    public interface IEmployerService
    {
        Task AddEmployerAsync(Employer employer);
        Task<Employer?> GetByUserIdAsync(string userId);
        Task<Employer?> GetEmployerByEmailAsync(string email);
        Task<IEnumerable<Employer>> GetAllEmployersAsync();
        Task<Employer?> GetEmployerProfileAsync(string userId);
        Task EditEmployerProfile(EditEmployerViewModel model, Employer employer);
    }
}
