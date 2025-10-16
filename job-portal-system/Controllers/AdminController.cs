using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _adminService.GetTotalUsersAsync();
            var totalEmployers = await _adminService.GetTotalEmployersAsync();
            var totalJobs = await _adminService.GetTotalJobsAsync();
            var pendingEmployers = await _adminService.GetPendingEmployersAsync();

            ViewBag.TotalUsers = totalUsers;
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
