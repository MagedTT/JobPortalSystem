using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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
