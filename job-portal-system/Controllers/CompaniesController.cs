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
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CompaniesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Companies
        public async Task<IActionResult> Index(string search, string location)
        {
            var employersQuery = _context.Employers
                .Include(e => e.User)
                .Where(e => e.IsApproved == true)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                employersQuery = employersQuery.Where(e => 
                    e.CompanyName.Contains(search) || 
                    e.Description.Contains(search));
            }

            // Apply location filter
            if (!string.IsNullOrEmpty(location))
            {
                employersQuery = employersQuery.Where(e => e.Location.Contains(location));
            }

            var employers = await employersQuery
                .OrderBy(e => e.CompanyName)
                .ToListAsync();

            var viewModels = new List<CompanyViewModel>();

            foreach (var employer in employers)
            {
                var jobCount = await _context.Jobs
                    .Where(j => j.EmployerId == employer.Id && j.IsActive)
                    .CountAsync();

                viewModels.Add(new CompanyViewModel
                {
                    Id = employer.Id,
                    CompanyName = employer.CompanyName,
                    Description = employer.Description,
                    Location = employer.Location,
                    Website = null, // Not in model
                    Industry = null, // Not in model
                    CompanySize = null, // Not in model
                    ActiveJobsCount = jobCount,
                    UserId = employer.UserId
                });
            }

            return View(viewModels);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employer == null)
            {
                return NotFound();
            }

            var jobs = await _context.Jobs
                .Include(j => j.Category)
                .Where(j => j.EmployerId == id && j.IsActive)
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();

            var jobViewModels = jobs.Select(j => new JobViewModel
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Salary = j.Salary,
                Location = j.Location,
                YearsOfExperience = j.YearsOfExperience,
                JobType = j.JobType.ToString(),
                CategoryName = j.Category?.Name ?? "",
                CompanyName = employer.CompanyName,
                CreatedAt = j.PostedAt,
                IsActive = j.IsActive
            }).ToList();

            var viewModel = new CompanyDetailsViewModel
            {
                Id = employer.Id,
                CompanyName = employer.CompanyName,
                Description = employer.Description,
                Location = employer.Location,
                Website = null, // Not in model
                Industry = null, // Not in model
                CompanySize = null, // Not in model
                UserId = employer.UserId,
                Jobs = jobViewModels
            };

            return View(viewModel);
        }

        // GET: Companies/SendMessage/5
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> SendMessage(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employer == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SendMessageViewModel
            {
                ReceiverId = employer.UserId,
                ReceiverName = employer.CompanyName,
                SenderId = jobSeeker.UserId
            };

            return View(viewModel);
        }

        // POST: Companies/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var message = new Message
            {
                SenderId = userId,
                ReceiverId = model.ReceiverId,
                Content = $"Subject: {model.Subject}\n\n{model.Content}",
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message sent successfully!";
            
            // Get employer ID from UserId
            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserId == model.ReceiverId);
            
            if (employer != null)
            {
                return RedirectToAction(nameof(Details), new { id = employer.Id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
