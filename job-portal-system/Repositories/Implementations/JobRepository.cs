using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Job>> GetRecentJobsAsync(int count)
        {
            return await _context.Jobs
                .OrderByDescending(j => j.PostedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
