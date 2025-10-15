using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersAsync()
            => await _context.Users.CountAsync();

        public async Task<int> GetTotalEmployersAsync()
            => await _context.Employers.CountAsync();

        public async Task<int> GetTotalJobsAsync()
            => await _context.Jobs.CountAsync();

        public async Task<int> GetPendingEmployersAsync()
            => await _context.Employers.CountAsync(e => !e.IsApproved);

        public async Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync()
            => await _context.Employers
                .Include(e => e.User)
                .Where(e => !e.IsApproved)
                .ToListAsync();

        public async Task ApproveEmployerAsync(string employerId)
        {
            var employer = await _context.Employers.FindAsync(employerId);
            if (employer == null) return;
            employer.IsApproved = true;
            await _context.SaveChangesAsync();
        }
    }
}
