using AutoMapper;
using job_portal_system.Attributes;
using job_portal_system.Models.DTOs;
using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    }
}
