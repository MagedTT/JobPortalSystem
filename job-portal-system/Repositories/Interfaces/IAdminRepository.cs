using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalEmployersAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetPendingEmployersAsync();
        Task<int> GetPendingJobSeekersCountAsync();
        Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync(int page = 1, int pageSize = 10);
        Task ApproveEmployerAsync(string employerId);
        Task<IEnumerable<JobSeeker>> GetPendingJobSeekersAsync(int page = 1, int pageSize = 10);
        Task ApproveJobSeekerAsync(string jobSeekerId);
        
        // Reports
        Task<List<UserReportDto>> GetUserReportsAsync();
        Task<List<JobReportDto>> GetJobReportsAsync();
        Task<List<ApplicationReportDto>> GetApplicationReportsAsync();
        Task<List<EmployerReportDto>> GetEmployerReportsAsync();
        Task<List<JobSeekerReportDto>> GetJobSeekerReportsAsync();
    }
}
