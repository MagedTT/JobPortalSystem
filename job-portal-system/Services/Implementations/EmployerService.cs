using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using job_portal_system.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace job_portal_system.Services.Implementations
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;

        public EmployerService(IEmployerRepository employerRepository)
        {
            _employerRepository = employerRepository;
        }

        public async Task AddEmployerAsync(Employer employer)
        {
            await _employerRepository.AddAsync(employer);
            await _employerRepository.SaveAsync();
        }

        public async Task<Employer?> GetByUserIdAsync(string userId)
        {
            return await _employerRepository.GetByUserIdAsync(userId);
        }

        public async Task<Employer?> GetEmployerByEmailAsync(string email)
        {
            return await _employerRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
        {
            return await _employerRepository.GetAllAsync();
        }

        public async Task UpdateEmployerAsync(Employer employer)
        {
            _employerRepository.Update(employer);
            await _employerRepository.SaveAsync();
        }
    }
}
