using job_portal_system.Data;
using job_portal_system.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public EmployerController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Employer/Dashboard
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.UserId == currentUser.Id);

        if (employer == null)
        {
            TempData["Error"] = "Employer profile not found.";
            return RedirectToAction("Index", "Home");
        }

        // Get employer's jobs
        var employerJobs = await _context.Jobs
            .Where(j => j.EmployerId == employer.Id)
            .ToListAsync();

        var jobIds = employerJobs.Select(j => j.Id).ToList();

        // Get statistics
        var totalJobs = employerJobs.Count;
        var activeJobs = employerJobs.Count(j => j.IsActive);
        
        var totalApplications = await _context.Applications
            .Where(a => jobIds.Contains(a.JobId))
            .CountAsync();

        var pendingApplications = await _context.Applications
            .Where(a => jobIds.Contains(a.JobId) && a.Status == Status.Pending)
            .CountAsync();

        var acceptedApplications = await _context.Applications
            .Where(a => jobIds.Contains(a.JobId) && a.Status == Status.Accepted)
            .CountAsync();

        var rejectedApplications = await _context.Applications
            .Where(a => jobIds.Contains(a.JobId) && a.Status == Status.Rejected)
            .CountAsync();

        // Recent applications (last 5)
        var recentApplications = await _context.Applications
            .Where(a => jobIds.Contains(a.JobId))
            .Include(a => a.Job)
            .Include(a => a.JobSeeker)
            .OrderByDescending(a => a.ApplicationDate)
            .Take(5)
            .Select(a => new
            {
                ApplicationId = a.Id,
                JobTitle = a.Job.Title,
                ApplicantName = a.JobSeeker.FirstName + " " + a.JobSeeker.LastName,
                ApplicationDate = a.ApplicationDate,
                Status = a.Status.ToString()
            })
            .ToListAsync();

        ViewBag.TotalJobs = totalJobs;
        ViewBag.ActiveJobs = activeJobs;
        ViewBag.TotalApplications = totalApplications;
        ViewBag.PendingApplications = pendingApplications;
        ViewBag.AcceptedApplications = acceptedApplications;
        ViewBag.RejectedApplications = rejectedApplications;
        ViewBag.RecentApplications = recentApplications;
        ViewBag.EmployerName = employer.CompanyName;

        return View();
    }

    // GET: Employer/Applicants
    [HttpGet]
    public async Task<IActionResult> Applicants(string? status, int? jobId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.UserId == currentUser.Id);

        if (employer == null)
        {
            TempData["Error"] = "Employer profile not found.";
            return RedirectToAction("Index", "Home");
        }

        // Get employer's jobs
        var employerJobs = await _context.Jobs
            .Where(j => j.EmployerId == employer.Id)
            .ToListAsync();

        var jobIds = employerJobs.Select(j => j.Id).ToList();

        // Query applications
        var applicationsQuery = _context.Applications
            .Where(a => jobIds.Contains(a.JobId))
            .Include(a => a.Job)
            .Include(a => a.JobSeeker)
                .ThenInclude(js => js.User)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<Status>(status, out var statusEnum))
            {
                applicationsQuery = applicationsQuery.Where(a => a.Status == statusEnum);
            }
        }

        // Filter by job
        if (jobId.HasValue)
        {
            applicationsQuery = applicationsQuery.Where(a => a.JobId == jobId.Value);
        }

        var applications = await applicationsQuery
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();

        ViewBag.EmployerJobs = employerJobs;
        ViewBag.SelectedStatus = status;
        ViewBag.SelectedJobId = jobId;

        return View(applications);
    }

    // POST: Employer/UpdateApplicationStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateApplicationStatus(int id, string status)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            TempData["Error"] = "Application not found.";
            return RedirectToAction(nameof(Applicants));
        }

        // Verify this application belongs to employer's job
        var currentUser = await _userManager.GetUserAsync(User);
        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.UserId == currentUser!.Id);

        if (employer == null || application.Job.EmployerId != employer.Id)
        {
            TempData["Error"] = "Unauthorized action.";
            return RedirectToAction(nameof(Applicants));
        }

        // Update status
        if (Enum.TryParse<Status>(status, out var statusEnum))
        {
            application.Status = statusEnum;
            application.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Application status updated to {status}.";
        }
        else
        {
            TempData["Error"] = "Invalid status.";
        }

        return RedirectToAction(nameof(Applicants));
    }
}
