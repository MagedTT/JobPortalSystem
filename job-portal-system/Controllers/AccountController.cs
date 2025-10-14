using job_portal_system.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register() => View("Index");

        [HttpGet]
        public IActionResult RegisterJobSeeker() => View(new RegisterJobSeekerViewModel());

        [HttpGet]
        public IActionResult RegisterEmployeer() => View(new RegisterEmployeerViewModel());

        [HttpPost]
        public IActionResult RegisterJobSeeker(RegisterJobSeekerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Logic for creating JobSeeker user here
            TempData["Message"] = "Job Seeker registered successfully!";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult RegisterEmployeer(RegisterEmployeerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Logic for creating Employer user here
            TempData["Message"] = "Employer registered successfully!";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        public IActionResult Profile() => View();
        public IActionResult Logout()
        {
            // sign-out logic here
            return RedirectToAction("Index", "Home");
        }
    }
}
