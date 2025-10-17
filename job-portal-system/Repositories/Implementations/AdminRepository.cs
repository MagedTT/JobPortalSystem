using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetTotalUsersAsync()
            => await _context.Users.CountAsync();

        public async Task<int> GetTotalEmployersAsync()
            => await _context.Employers.CountAsync();

        public async Task<int> GetTotalJobsAsync()
            => await _context.Jobs.CountAsync();

        public async Task<int> GetPendingEmployersAsync()
            => await _context.Employers.CountAsync(e => !e.IsApproved);

        public async Task<int> GetPendingJobSeekersCountAsync()
            => await _context.JobSeekers.CountAsync(js => !js.IsApproved);

        public async Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Employers
                .Include(e => e.User)
                .Where(e => !e.IsApproved)
                .OrderBy(e => e.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task ApproveEmployerAsync(string employerId)
        {
            var employer = await _context.Employers.FindAsync(employerId);
            if (employer == null) return;
            employer.IsApproved = true;
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<JobSeeker>> GetPendingJobSeekersAsync(int page = 1, int pageSize = 10)
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .Where(js => js.IsApproved == false)
                .OrderBy(js => js.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task ApproveJobSeekerAsync(string jobSeekerId)
        {
            var jobSeeker = await _context.JobSeekers.FindAsync(jobSeekerId);
            if (jobSeeker == null) return;
            jobSeeker.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        // ==================== Reports ====================

        public async Task<List<UserReportDto>> GetUserReportsAsync()
        {
            var users = await _context.Users.ToListAsync();
            var reports = new List<UserReportDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                reports.Add(new UserReportDto
                {
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "Unknown",
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedDate = null // You can add a CreatedDate property to User model if needed
                });
            }

            return reports;
        }

        public async Task<List<JobReportDto>> GetJobReportsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .Select(j => new JobReportDto
                {
                    Title = j.Title,
                    Company = j.Employer.CompanyName,
                    Category = j.Category.Name,
                    Location = j.Location,
                    Salary = j.Salary,
                    JobType = j.JobType.ToString(),
                    ApplicationsCount = j.Applications.Count,
                    PostedAt = j.PostedAt,
                    IsActive = j.IsActive
                })
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();
        }

        public async Task<List<ApplicationReportDto>> GetApplicationReportsAsync()
        {
            return await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Select(a => new ApplicationReportDto
                {
                    JobTitle = a.Job.Title,
                    Company = a.Job.Employer.CompanyName,
                    ApplicantName = a.JobSeeker.FirstName + " " + a.JobSeeker.LastName,
                    ApplicantEmail = a.JobSeeker.User.Email ?? "",
                    Status = a.Status.ToString(),
                    AppliedDate = a.ApplicationDate,
                    ExperienceYears = a.JobSeeker.ExperienceYears
                })
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();
        }

        public async Task<List<EmployerReportDto>> GetEmployerReportsAsync()
        {
            return await _context.Employers
                .Include(e => e.User)
                .Include(e => e.Jobs)
                .Select(e => new EmployerReportDto
                {
                    CompanyName = e.CompanyName,
                    Email = e.User.Email ?? "",
                    Location = e.Location,
                    IsApproved = e.IsApproved,
                    TotalJobs = e.Jobs.Count,
                    ActiveJobs = e.Jobs.Count(j => j.IsActive),
                    RegisteredDate = DateTime.Now // You can add a CreatedDate property if needed
                })
                .OrderBy(e => e.CompanyName)
                .ToListAsync();
        }

        public async Task<List<JobSeekerReportDto>> GetJobSeekerReportsAsync()
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .Include(js => js.Applications)
                .Select(js => new JobSeekerReportDto
                {
                    FullName = js.FirstName + " " + js.LastName,
                    Email = js.User.Email ?? "",
                    Education = js.Education,
                    ExperienceYears = js.ExperienceYears,
                    Location = js.Location,
                    IsApproved = js.IsApproved,
                    TotalApplications = js.Applications.Count,
                    RegisteredDate = DateTime.Now // You can add a CreatedDate property if needed
                })
                .OrderBy(js => js.FullName)
                .ToListAsync();
        }
    }
}
