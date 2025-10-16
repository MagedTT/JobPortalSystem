using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IJobSeekerService _jobSeekerService;

        public AdminController(IAdminService adminService, IJobSeekerService jobSeekerService)
        {
            _adminService = adminService;
            _jobSeekerService = jobSeekerService;
        }

        public async Task<IActionResult> CreateNewJobSeeker()
        {
            return View(new RegisterJobSeekerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewJobSeeker(RegisterJobSeekerViewModel model)
        {
            await _jobSeekerService.AddJobSeekerMagedAsync(model);
            return RedirectToAction(nameof(ManageUsers));
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _adminService.GetTotalJobSeekersAsync();
            var totalEmployers = await _adminService.GetTotalEmployersAsync();
            var totalJobs = await _adminService.GetTotalJobsAsync();
            var pendingEmployers = await _adminService.GetPendingEmployersAsync();
            var allSalaries = await _adminService.GetAllSalariesForAllJobs();
            var allCategoriesWithCount = await _adminService.GetJobsCountByCategoriesAsync();


            ViewBag.AllSalaries = allSalaries;
            ViewBag.AllCategories = allCategoriesWithCount.Keys.ToList();
            ViewBag.CategoryCounts = allCategoriesWithCount.Values.ToList();
            ViewBag.TotalJobSeekers = totalUsers;
            ViewBag.TotalEmployers = totalEmployers;
            ViewBag.TotalJobs = totalJobs;
            ViewBag.PendingEmployers = pendingEmployers;

            return View();
        }

        public async Task<IActionResult> ManageUsers(int page = 1, string email = null!)
        {
            var (jobSeekers, totalPages) = await _jobSeekerService.DiplayJobSeekerForAdminViewModel(page: page, email: email);
            ViewBag.TotalPages = totalPages;
            return View(jobSeekers);
        }

        public async Task<IActionResult> PendingEmployers()
        {
            var employers = await _adminService.GetUnapprovedEmployersAsync();
            return View(employers);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveEmployer(string id)
        {
            await _adminService.ApproveEmployerAsync(id);
            return RedirectToAction("PendingEmployers");
        }
    }
}
