using job_portal_system.Models.Domain;

namespace job_portal_system.Services.Interfaces
{
    public interface IEmployerService
    {
        Task AddEmployerAsync(Employer employer);
        Task<Employer?> GetByUserIdAsync(string userId);
        Task<Employer?> GetEmployerByEmailAsync(string email);
        Task<IEnumerable<Employer>> GetAllEmployersAsync();
        Task UpdateEmployerAsync(Employer employer);
    }
}
