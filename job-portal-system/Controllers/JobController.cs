using AutoMapper;
using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.DTOs;
using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace job_portal_system.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public JobController(IJobService jobService, IMapper mapper, ApplicationDbContext context, UserManager<User> userManager)
        {
            _jobService = jobService;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search, string location, string jobType)
        {
            var jobDtos = await _jobService.GetAllJobsAsync();
            var viewModels = _mapper.Map<IEnumerable<JobViewModel>>(jobDtos);
            
            // Get current user's applications if they're a job seeker
            if (User.IsInRole("JobSeeker"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.UserId == userId);
                
                if (jobSeeker != null)
                {
                    var appliedJobIds = await _context.Applications
                        .Where(a => a.JobSeekerId == jobSeeker.Id)
                        .Select(a => a.JobId)
                        .ToListAsync();

                    foreach (var job in viewModels)
                    {
                        job.HasApplied = appliedJobIds.Contains(job.Id);
                    }
                }
            }

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                viewModels = viewModels.Where(j => 
                    j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    j.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    j.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(location))
            {
                viewModels = viewModels.Where(j => j.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(jobType))
            {
                viewModels = viewModels.Where(j => j.JobType == jobType);
            }

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound();

            var viewModel = new JobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                Location = job.Location,
                YearsOfExperience = job.YearsOfExperience,
                JobType = job.JobType.ToString(),
                CategoryName = job.Category?.Name ?? "",
                CompanyName = job.Employer?.CompanyName ?? "",
                EmployerId = job.EmployerId,
                CreatedAt = job.PostedAt,
                IsActive = job.IsActive
            };

            // Check if user has applied
            if (User.IsInRole("JobSeeker"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.UserId == userId);
                
                if (jobSeeker != null)
                {
                    viewModel.HasApplied = await _context.Applications
                        .AnyAsync(a => a.JobId == id && a.JobSeekerId == jobSeeker.Id);
                }
            }

            return View(viewModel);
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if already applied
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == id && a.JobSeekerId == jobSeeker.Id);

            if (existingApplication != null)
            {
                TempData["Info"] = "You have already applied for this job.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new ApplyJobViewModel
            {
                JobId = id,
                JobTitle = job.Title,
                CompanyName = job.Employer?.CompanyName ?? "",
                JobSeekerId = jobSeeker.Id
            };

            return View(viewModel);
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyJobViewModel model, IFormFile? cvFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if already applied
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == model.JobId && a.JobSeekerId == jobSeeker.Id);

            if (existingApplication != null)
            {
                TempData["Info"] = "You have already applied for this job.";
                return RedirectToAction(nameof(Details), new { id = model.JobId });
            }

            // Handle CV upload
            string cvPath = "";
            if (cvFile != null && cvFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(cvFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }
                
                cvPath = $"/uploads/cvs/{uniqueFileName}";
            }

            // Create application
            var application = new Application
            {
                JobId = model.JobId,
                JobSeekerId = jobSeeker.Id,
                CoverLetter = model.CoverLetter ?? "",
                CVPath = cvPath,
                Status = Status.Pending,
                ApplicationDate = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your application has been submitted successfully!";
            return RedirectToAction(nameof(Details), new { id = model.JobId });
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> ManageJobs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employer == null)
            {
                TempData["Error"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var jobs = await _context.Jobs
                .Where(j => j.EmployerId == employer.Id)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();

            var viewModels = jobs.Select(job => new JobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                Location = job.Location,
                YearsOfExperience = job.YearsOfExperience,
                JobType = job.JobType.ToString(),
                CategoryName = job.Category?.Name ?? "",
                CompanyName = employer.CompanyName,
                EmployerId = job.EmployerId,
                CreatedAt = job.PostedAt,
                IsActive = job.IsActive,
                ApplicationsCount = job.Applications.Count
            }).ToList();

            return View(viewModels);
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create()
        {
            // Get categories for dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.JobTypes = Enum.GetValues(typeof(JobType)).Cast<JobType>().ToList();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create(JobViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.JobTypes = Enum.GetValues(typeof(JobType)).Cast<JobType>().ToList();
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employer == null)
            {
                TempData["Error"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var job = new Job
            {
                EmployerId = employer.Id,
                Title = vm.Title,
                Description = vm.Description,
                Salary = (int)vm.Salary,
                Location = vm.Location,
                YearsOfExperience = vm.YearsOfExperience,
                JobType = Enum.Parse<JobType>(vm.JobType),
                CategoryId = vm.CategoryId,
                PostedAt = DateTime.Now,
                IsActive = true
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job posted successfully!";
            return RedirectToAction(nameof(ManageJobs));
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound();

            // Verify ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            
            if (employer == null || job.EmployerId != employer.Id)
            {
                TempData["Error"] = "You don't have permission to edit this job.";
                return RedirectToAction(nameof(ManageJobs));
            }

            var vm = new JobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                Location = job.Location,
                YearsOfExperience = job.YearsOfExperience,
                JobType = job.JobType.ToString(),
                CategoryName = job.Category?.Name ?? "",
                CompanyName = job.Employer?.CompanyName ?? "",
                EmployerId = job.EmployerId,
                IsActive = job.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(JobViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var job = await _context.Jobs.FindAsync(vm.Id);
            if (job == null)
                return NotFound();

            // Verify ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            
            if (employer == null || job.EmployerId != employer.Id)
            {
                TempData["Error"] = "You don't have permission to edit this job.";
                return RedirectToAction(nameof(ManageJobs));
            }

            // Update job properties
            job.Title = vm.Title;
            job.Description = vm.Description;
            job.Salary = (int)vm.Salary;
            job.Location = vm.Location;
            job.YearsOfExperience = vm.YearsOfExperience;
            job.JobType = Enum.Parse<JobType>(vm.JobType);
            job.IsActive = vm.IsActive;

            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Job updated successfully!";
            return RedirectToAction(nameof(ManageJobs));
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound();

            // Verify ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            
            if (employer == null || job.EmployerId != employer.Id)
            {
                TempData["Error"] = "You don't have permission to delete this job.";
                return RedirectToAction(nameof(ManageJobs));
            }

            var vm = new JobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                Location = job.Location,
                CompanyName = job.Employer?.CompanyName ?? ""
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound();

            // Verify ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            
            if (employer == null || job.EmployerId != employer.Id)
            {
                TempData["Error"] = "You don't have permission to delete this job.";
                return RedirectToAction(nameof(ManageJobs));
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Job deleted successfully!";
            return RedirectToAction(nameof(ManageJobs));
        }

        public async Task<IActionResult> RecentJobs(int count = 5)
        {
            var jobDtos = await _jobService.GetRecentJobsAsync(count);
            var vms = _mapper.Map<IEnumerable<JobViewModel>>(jobDtos);
            return PartialView("_RecentJobsPartial", vms);
        }
    }
}
