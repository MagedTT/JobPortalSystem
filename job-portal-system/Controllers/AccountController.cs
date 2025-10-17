using AutoMapper;
using job_portal_system.Attributes;
using job_portal_system.Models.Domain;
using job_portal_system.Models.DTOs;
using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IJobSeekerService _jobSeekerService;
        private readonly IEmployerService _employerService;
        private readonly IAdminService _adminService;
        private readonly UserManager<User> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public AccountController(
            IAuthService authService, 
            IMapper mapper,
            IJobSeekerService jobSeekerService,
            IEmployerService employerService,
            IAdminService adminService,
            UserManager<User> userManager,
            IFileUploadService fileUploadService)
        {
            _authService = authService;
            _mapper = mapper;
            _jobSeekerService = jobSeekerService;
            _employerService = employerService;
            _adminService = adminService;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        // ==================== Register Views ====================

        [HttpGet]
        [RedirectAuthenticated]
        public IActionResult Register() => View("Index");

        [HttpGet]
        [RedirectAuthenticated]
        public IActionResult RegisterJobSeeker() => View(new RegisterJobSeekerViewModel());

        [HttpGet]
        [RedirectAuthenticated]
        public IActionResult RegisterEmployer() => View(new RegisterEmployerViewModel());


        // ==================== Register Job Seeker ====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterJobSeeker(RegisterJobSeekerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<RegisterJobSeekerDto>(model);

            var (isSuccess, error) = await _authService.RegisterJobSeekerAsync(dto);

            if (isSuccess)
            {
                TempData["Message"] = "Job Seeker registered successfully!";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", error);
            return View(model);
        }


        // ==================== Register Employer ====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployer(RegisterEmployerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<RegisterEmployerDto>(model);

            var (isSuccess, error) = await _authService.RegisterEmployerAsync(dto);

            if (isSuccess)
            {
                TempData["Message"] = "Employer registered successfully!";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", error);
            return View(model);
        }


        // ==================== Login ====================

        [HttpGet]
        [RedirectAuthenticated]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectAuthenticated]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (isSuccess, error) = await _authService.LoginAsync(model.Email, model.Password);

            if (isSuccess)
            {
                // Redirect based on user role
                if (User.IsInRole("Employer"))
                {
                    return RedirectToAction("Dashboard", "Employer");
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", error);
            return View(model);
        }


        // ==================== Profile ====================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            if (User.IsInRole("Admin"))
            {
                var adminProfile = new AdminProfileViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    TotalUsers = await _adminService.GetTotalUsersAsync(),
                    TotalEmployers = await _adminService.GetTotalEmployersAsync(),
                    TotalJobs = await _adminService.GetTotalJobsAsync(),
                    PendingEmployers = await _adminService.GetPendingEmployersAsync(),
                    PendingJobSeekers = await _adminService.GetPendingJobSeekersCountAsync()
                };
                adminProfile.TotalJobSeekers = adminProfile.TotalUsers - adminProfile.TotalEmployers - 1; // Exclude admin
                return View("AdminProfile", adminProfile);
            }
            else if (User.IsInRole("Employer"))
            {
                var employer = await _employerService.GetByUserIdAsync(user.Id);
                if (employer == null)
                    return RedirectToAction("Login");

                var employerProfile = new EmployerProfileViewModel
                {
                    Id = employer.Id,
                    UserId = employer.UserId,
                    CompanyName = employer.CompanyName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    Description = employer.Description,
                    Location = employer.Location,
                    LogoPath = employer.LogoPath,
                    IsApproved = employer.IsApproved,
                    TotalJobs = employer.Jobs?.Count ?? 0
                };
                return View("EmployerProfile", employerProfile);
            }
            else if (User.IsInRole("JobSeeker"))
            {
                var jobSeeker = await _jobSeekerService.GetByUserIdAsync(user.Id);
                if (jobSeeker == null)
                    return RedirectToAction("Login");

                var jobSeekerProfile = new JobSeekerProfileViewModel
                {
                    Id = jobSeeker.Id,
                    UserId = jobSeeker.UserId,
                    FirstName = jobSeeker.FirstName,
                    LastName = jobSeeker.LastName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    Education = jobSeeker.Education,
                    ExperienceYears = jobSeeker.ExperienceYears,
                    Location = jobSeeker.Location,
                    Skills = jobSeeker.Skills,
                    ImagePath = jobSeeker.ImagePath,
                    CVPath = jobSeeker.CVPath,
                    IsApproved = jobSeeker.IsApproved
                };
                return View("JobSeekerProfile", jobSeekerProfile);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize(Roles = "JobSeeker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJobSeekerProfile(JobSeekerProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("JobSeekerProfile", model);

            var jobSeeker = await _jobSeekerService.GetByUserIdAsync(model.UserId);
            if (jobSeeker == null)
                return NotFound();

            // Handle profile image upload
            if (model.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(jobSeeker.ImagePath))
                {
                    await _fileUploadService.DeleteFileAsync(jobSeeker.ImagePath);
                }

                var imagePath = await _fileUploadService.UploadImageAsync(model.ImageFile, "profiles");
                if (imagePath != null)
                {
                    jobSeeker.ImagePath = imagePath;
                }
            }

            // Handle CV upload
            if (model.CVFile != null)
            {
                // Delete old CV if exists
                if (!string.IsNullOrEmpty(jobSeeker.CVPath))
                {
                    await _fileUploadService.DeleteFileAsync(jobSeeker.CVPath);
                }

                var cvPath = await _fileUploadService.UploadFileAsync(model.CVFile, "cvs");
                if (cvPath != null)
                {
                    jobSeeker.CVPath = cvPath;
                }
            }

            jobSeeker.FirstName = model.FirstName;
            jobSeeker.LastName = model.LastName;
            jobSeeker.Education = model.Education;
            jobSeeker.ExperienceYears = model.ExperienceYears;
            jobSeeker.Location = model.Location;
            jobSeeker.Skills = model.Skills;

            await _jobSeekerService.UpdateJobSeekerAsync(jobSeeker);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployerProfile(EmployerProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("EmployerProfile", model);

            var employer = await _employerService.GetByUserIdAsync(model.UserId);
            if (employer == null)
                return NotFound();

            // Handle logo upload
            if (model.LogoFile != null)
            {
                // Delete old logo if exists
                if (!string.IsNullOrEmpty(employer.LogoPath))
                {
                    await _fileUploadService.DeleteFileAsync(employer.LogoPath);
                }

                var logoPath = await _fileUploadService.UploadImageAsync(model.LogoFile, "logos");
                if (logoPath != null)
                {
                    employer.LogoPath = logoPath;
                }
            }

            employer.CompanyName = model.CompanyName;
            employer.Description = model.Description;
            employer.Location = model.Location;

            await _employerService.UpdateEmployerAsync(employer);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdminProfile(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("AdminProfile", model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            // Update username and phone number
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("AdminProfile", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }


        // ==================== Security (Change Password) ====================

        [HttpGet]
        [Authorize]
        public IActionResult Security()
        {
            return View(new ChangePasswordDto());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Security(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (isSuccess, error) = await _authService.ChangePasswordAsync(User, model.CurrentPassword, model.NewPassword);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Security");
            }

            ModelState.AddModelError("", error);
            return View(model);
        }


        // ==================== Logout ====================

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var callbackUrl = Url.Action("ResetPassword", "Account", null, Request.Scheme);
            var result = await _authService.ForgotPasswordAsync(model.Email, callbackUrl);

            if (!result.IsSuccess)
            {
                ViewBag.Error = result.ErrorMessage;
                return View(model);
            }

            ViewBag.Message = "A reset link has been sent to your email.";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordDto { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

            if (!result.IsSuccess)
            {
                ViewBag.Error = result.ErrorMessage;
                return View(model);
            }

            ViewBag.Message = "Password has been reset successfully!";
            return View("ResetPasswordConfirmation");
        }
    }
}
