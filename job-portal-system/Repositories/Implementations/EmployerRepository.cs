using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class EmployerRepository : GenericRepository<Employer>, IEmployerRepository
    {
        public EmployerRepository(ApplicationDbContext context) : base(context) {}

        public async Task<Employer?> GetByEmailAsync(string email)
        {
            return await _context.Employers
                .Include(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.User.Email == email);
        }

        public async Task<Employer?> GetByUserIdAsync(string userId)
        {
            return await _context.Employers
                .Include(e => e.User)
                .Include(e => e.Jobs)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

    }
}
