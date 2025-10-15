using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using job_portal_system.Services.Interfaces;

namespace job_portal_system.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public Task<int> GetTotalUsersAsync() => _adminRepository.GetTotalUsersAsync();
        public Task<int> GetTotalEmployersAsync() => _adminRepository.GetTotalEmployersAsync();
        public Task<int> GetTotalJobsAsync() => _adminRepository.GetTotalJobsAsync();
        public Task<int> GetPendingEmployersAsync() => _adminRepository.GetPendingEmployersAsync();
        public Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync() => _adminRepository.GetUnapprovedEmployersAsync();
        public Task ApproveEmployerAsync(string employerId) => _adminRepository.ApproveEmployerAsync(employerId);
    }
}
