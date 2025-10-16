using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class AdminRepository : JobSeekerRepository, IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminRepository(ApplicationDbContext context, IWebHostEnvironment env, UserManager<User> userManager) : base(context, env, userManager)
        {
            _context = context;
        }

        public async Task<int> GetTotalJobSeekersAsync()
            => await _context.JobSeekers.CountAsync();

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

        public async Task<List<int>> GetAllSalariesForAllJobs()
            => await _context.Jobs.Select(x => x.Salary).ToListAsync();

        public async Task<Dictionary<string, int>> GetJobsCountByCategoriesAsync()
            => await _context.Jobs.GroupBy(x => x.Category).Select(r => new { Category = r.Key, Count = r.Count() }).ToDictionaryAsync(x => x.Category.Name, x => x.Count);
    }
}
