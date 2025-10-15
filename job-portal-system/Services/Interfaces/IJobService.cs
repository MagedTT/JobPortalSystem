using job_portal_system.Models.DTOs;

namespace job_portal_system.Services.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<JobDto>> GetAllJobsAsync();
        Task<JobDto?> GetJobByIdAsync(string id);
        Task<IEnumerable<JobDto>> GetRecentJobsAsync(int count);
        Task AddJobAsync(JobDto dto);
        Task UpdateJobAsync(JobDto dto);
        Task DeleteJobAsync(string id);
    }
}
