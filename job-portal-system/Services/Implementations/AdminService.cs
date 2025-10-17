using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
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
        public Task<int> GetPendingJobSeekersCountAsync() => _adminRepository.GetPendingJobSeekersCountAsync();
        public Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync(int page = 1, int pageSize = 10) => _adminRepository.GetUnapprovedEmployersAsync(page, pageSize);
        public Task ApproveEmployerAsync(string employerId) => _adminRepository.ApproveEmployerAsync(employerId);

        public Task<IEnumerable<JobSeeker>> GetPendingJobSeekersAsync(int page = 1, int pageSize = 10) => _adminRepository.GetPendingJobSeekersAsync(page, pageSize);
        public Task ApproveJobSeekerAsync(string jobSeekerId) => _adminRepository.ApproveJobSeekerAsync(jobSeekerId);

        // ==================== Reports ====================
        
        public Task<List<UserReportDto>> GetUserReportsAsync() => _adminRepository.GetUserReportsAsync();
        public Task<List<JobReportDto>> GetJobReportsAsync() => _adminRepository.GetJobReportsAsync();
        public Task<List<ApplicationReportDto>> GetApplicationReportsAsync() => _adminRepository.GetApplicationReportsAsync();
        public Task<List<EmployerReportDto>> GetEmployerReportsAsync() => _adminRepository.GetEmployerReportsAsync();
        public Task<List<JobSeekerReportDto>> GetJobSeekerReportsAsync() => _adminRepository.GetJobSeekerReportsAsync();
    }
}
