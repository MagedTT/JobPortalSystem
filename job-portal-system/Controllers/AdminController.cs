using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace job_portal_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _adminService.GetTotalUsersAsync();
            var totalEmployers = await _adminService.GetTotalEmployersAsync();
            var totalJobs = await _adminService.GetTotalJobsAsync();
            var pendingEmployers = await _adminService.GetPendingEmployersAsync();
            var pendingJobSeekers = await _adminService.GetPendingJobSeekersCountAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalEmployers = totalEmployers;
            ViewBag.TotalJobs = totalJobs;
            ViewBag.PendingEmployers = pendingEmployers;
            ViewBag.PendingJobSeekers = pendingJobSeekers;

            return View();
        }


        public async Task<IActionResult> PendingEmployers(int employerPage = 1, int jobSeekerPage = 1, int pageSize = 10)
        {
            var employers = await _adminService.GetUnapprovedEmployersAsync(employerPage, pageSize);
            var jobSeekers = await _adminService.GetPendingJobSeekersAsync(jobSeekerPage, pageSize);
            var totalEmployers = await _adminService.GetPendingEmployersAsync();
            var totalJobSeekers = await _adminService.GetPendingJobSeekersCountAsync();
            
            var vm = new job_portal_system.Models.ViewModels.PendingEmployersViewModel
            {
                Employers = employers,
                JobSeekers = jobSeekers,
                EmployerPage = employerPage,
                JobSeekerPage = jobSeekerPage,
                PageSize = pageSize,
                TotalEmployers = totalEmployers,
                TotalJobSeekers = totalJobSeekers
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveJobSeeker(string id)
        {
            await _adminService.ApproveJobSeekerAsync(id);
            return RedirectToAction("PendingEmployers");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveEmployer(string id)
        {
            await _adminService.ApproveEmployerAsync(id);
            return RedirectToAction("PendingEmployers");
        }

        public IActionResult ManageJobs()
        {
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            var viewModel = new ReportsViewModel
            {
                UserReports = await _adminService.GetUserReportsAsync(),
                JobReports = await _adminService.GetJobReportsAsync(),
                ApplicationReports = await _adminService.GetApplicationReportsAsync(),
                EmployerReports = await _adminService.GetEmployerReportsAsync(),
                JobSeekerReports = await _adminService.GetJobSeekerReportsAsync()
            };

            return View(viewModel);
        }

        // ==================== Excel Exports ====================

        public async Task<IActionResult> ExportUsersToExcel()
        {
            var data = await _adminService.GetUserReportsAsync();
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users Report");

            // Headers
            worksheet.Cells[1, 1].Value = "Email";
            worksheet.Cells[1, 2].Value = "Role";
            worksheet.Cells[1, 3].Value = "Is Active";
            worksheet.Cells[1, 4].Value = "Email Confirmed";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].Email;
                worksheet.Cells[i + 2, 2].Value = data[i].Role;
                worksheet.Cells[i + 2, 3].Value = data[i].IsActive ? "Yes" : "No";
                worksheet.Cells[i + 2, 4].Value = data[i].EmailConfirmed ? "Yes" : "No";
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Users_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ExportJobsToExcel()
        {
            var data = await _adminService.GetJobReportsAsync();
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Jobs Report");

            // Headers
            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Company";
            worksheet.Cells[1, 3].Value = "Category";
            worksheet.Cells[1, 4].Value = "Location";
            worksheet.Cells[1, 5].Value = "Salary";
            worksheet.Cells[1, 6].Value = "Job Type";
            worksheet.Cells[1, 7].Value = "Applications";
            worksheet.Cells[1, 8].Value = "Posted Date";
            worksheet.Cells[1, 9].Value = "Is Active";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 9])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].Title;
                worksheet.Cells[i + 2, 2].Value = data[i].Company;
                worksheet.Cells[i + 2, 3].Value = data[i].Category;
                worksheet.Cells[i + 2, 4].Value = data[i].Location;
                worksheet.Cells[i + 2, 5].Value = data[i].Salary;
                worksheet.Cells[i + 2, 6].Value = data[i].JobType;
                worksheet.Cells[i + 2, 7].Value = data[i].ApplicationsCount;
                worksheet.Cells[i + 2, 8].Value = data[i].PostedAt.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 9].Value = data[i].IsActive ? "Yes" : "No";
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Jobs_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ExportApplicationsToExcel()
        {
            var data = await _adminService.GetApplicationReportsAsync();
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Applications Report");

            // Headers
            worksheet.Cells[1, 1].Value = "Job Title";
            worksheet.Cells[1, 2].Value = "Company";
            worksheet.Cells[1, 3].Value = "Applicant Name";
            worksheet.Cells[1, 4].Value = "Applicant Email";
            worksheet.Cells[1, 5].Value = "Status";
            worksheet.Cells[1, 6].Value = "Applied Date";
            worksheet.Cells[1, 7].Value = "Experience Years";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].JobTitle;
                worksheet.Cells[i + 2, 2].Value = data[i].Company;
                worksheet.Cells[i + 2, 3].Value = data[i].ApplicantName;
                worksheet.Cells[i + 2, 4].Value = data[i].ApplicantEmail;
                worksheet.Cells[i + 2, 5].Value = data[i].Status;
                worksheet.Cells[i + 2, 6].Value = data[i].AppliedDate.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 7].Value = data[i].ExperienceYears;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Applications_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ExportEmployersToExcel()
        {
            var data = await _adminService.GetEmployerReportsAsync();
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Employers Report");

            // Headers
            worksheet.Cells[1, 1].Value = "Company Name";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Location";
            worksheet.Cells[1, 4].Value = "Is Approved";
            worksheet.Cells[1, 5].Value = "Total Jobs";
            worksheet.Cells[1, 6].Value = "Active Jobs";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].CompanyName;
                worksheet.Cells[i + 2, 2].Value = data[i].Email;
                worksheet.Cells[i + 2, 3].Value = data[i].Location;
                worksheet.Cells[i + 2, 4].Value = data[i].IsApproved ? "Yes" : "No";
                worksheet.Cells[i + 2, 5].Value = data[i].TotalJobs;
                worksheet.Cells[i + 2, 6].Value = data[i].ActiveJobs;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Employers_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ExportJobSeekersToExcel()
        {
            var data = await _adminService.GetJobSeekerReportsAsync();
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("JobSeekers Report");

            // Headers
            worksheet.Cells[1, 1].Value = "Full Name";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Education";
            worksheet.Cells[1, 4].Value = "Experience Years";
            worksheet.Cells[1, 5].Value = "Location";
            worksheet.Cells[1, 6].Value = "Is Approved";
            worksheet.Cells[1, 7].Value = "Total Applications";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].FullName;
                worksheet.Cells[i + 2, 2].Value = data[i].Email;
                worksheet.Cells[i + 2, 3].Value = data[i].Education;
                worksheet.Cells[i + 2, 4].Value = data[i].ExperienceYears;
                worksheet.Cells[i + 2, 5].Value = data[i].Location;
                worksheet.Cells[i + 2, 6].Value = data[i].IsApproved ? "Yes" : "No";
                worksheet.Cells[i + 2, 7].Value = data[i].TotalApplications;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"JobSeekers_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public IActionResult ManageUsers()
        {
            return View();
        }
    }
}
