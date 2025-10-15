using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class JobSeekerRepository : GenericRepository<JobSeeker>, IJobSeekerRepository
    {
        public JobSeekerRepository(ApplicationDbContext context) : base(context) {}

        public async Task<JobSeeker?> GetByEmailAsync(string email)
        {
            return await _context.JobSeekers
                .Include(js => js.User) 
                .AsNoTracking()
                .FirstOrDefaultAsync(js => js.User.Email == email);
        }
    }
}
