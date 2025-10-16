using AutoMapper;
using job_portal_system.Attributes;
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

        public AccountController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
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
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", error);
            return View(model);
        }


        // ==================== Profile ====================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userProfile = await _authService.GetUserProfileAsync(User);
            if (userProfile == null)
                return RedirectToAction("Login");

            return View(userProfile);
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Security()
        {
            var user = await _authService.GetUserProfileAsync(User);
            if (user == null) return RedirectToAction("Login");

            var model = new SecurityViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Security(SecurityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _authService.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var securityDto = new SecurityDto
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            };

            var result = await _authService.UpdateSecurityAsync(user, securityDto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("CurrentPassword", result.ErrorMessage);
                return View(model);
            }

            TempData["Message"] = "Security settings updated successfully.";
            return RedirectToAction("Security");
        }
    }
}
