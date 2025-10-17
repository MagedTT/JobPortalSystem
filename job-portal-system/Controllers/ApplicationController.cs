using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace job_portal_system.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ApplicationController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var applications = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Category)
                .Where(a => a.JobSeekerId == jobSeeker.Id)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            var viewModels = applications.Select(a => new ApplicationViewModel
            {
                ApplicationId = a.Id,
                JobId = a.JobId,
                JobTitle = a.Job.Title,
                CompanyName = a.Job.Employer?.CompanyName ?? "N/A",
                JobLocation = a.Job.Location,
                Salary = a.Job.Salary,
                ApplicationDate = a.ApplicationDate,
                Status = a.Status.ToString(),
                CoverLetter = a.CoverLetter,
                CVPath = a.CVPath,
                UpdatedAt = a.UpdatedAt,
                JobType = a.Job.JobType.ToString(),
                CategoryName = a.Job.Category?.Name ?? "N/A"
            }).ToList();

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var application = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Category)
                .FirstOrDefaultAsync(a => a.Id == id && a.JobSeekerId == jobSeeker.Id);

            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction(nameof(MyApplications));
            }

            var viewModel = new ApplicationViewModel
            {
                ApplicationId = application.Id,
                JobId = application.JobId,
                JobTitle = application.Job.Title,
                CompanyName = application.Job.Employer?.CompanyName ?? "N/A",
                JobLocation = application.Job.Location,
                Salary = application.Job.Salary,
                ApplicationDate = application.ApplicationDate,
                Status = application.Status.ToString(),
                CoverLetter = application.CoverLetter,
                CVPath = application.CVPath,
                UpdatedAt = application.UpdatedAt,
                JobType = application.Job.JobType.ToString(),
                CategoryName = application.Job.Category?.Name ?? "N/A",
                JobDescription = application.Job.Description,
                YearsOfExperience = application.Job.YearsOfExperience
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == id && a.JobSeekerId == jobSeeker.Id);

            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction(nameof(MyApplications));
            }

            // Only allow withdrawal of pending applications
            if (application.Status != Status.Pending)
            {
                TempData["Error"] = "You can only withdraw pending applications.";
                return RedirectToAction(nameof(MyApplications));
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Application withdrawn successfully.";
            return RedirectToAction(nameof(MyApplications));
        }
    }
}
