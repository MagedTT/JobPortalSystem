using System.Threading.Tasks;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers;

public class EmployerController : Controller
{
    private readonly IEmployerService _employerService;
    private readonly IWebHostEnvironment _env;

    public EmployerController(IEmployerService employerService, IWebHostEnvironment env)
    {
        _employerService = employerService;
        _env = env;
    }

    public async Task<IActionResult> Index(string userId)
    {
        var employer = await _employerService.GetEmployerProfileAsync(userId);

        if (employer is null)
            return NotFound();

        EmployerViewModel model = new()
        {
            Id = employer.Id,
            UserId = employer.UserId,
            CompanyName = employer.CompanyName,
            Description = employer.Description,
            LogoPath = employer.LogoPath,
            Location = employer.Location,
            IsApproved = employer.IsApproved
        };

        return View(model);
    }

    public async Task<IActionResult> EditEmployerProfile(string userId)
    {
        var employer = await _employerService.GetEmployerProfileAsync(userId);

        if (employer is null)
            return NotFound();

        EditEmployerViewModel model = new()
        {
            Id = employer.Id,
            UserId = employer.UserId,
            CompanyName = employer.CompanyName,
            Description = employer.Description,
            LogoPath = employer.LogoPath,
            Location = employer.Location,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEmployerProfile(EditEmployerViewModel model)
    {
        var employer = await _employerService.GetEmployerProfileAsync(model.UserId);

        if (employer is null)
            return NotFound();

        await _employerService.EditEmployerProfile(model, employer);

        return RedirectToAction(nameof(Index), new { userId = model.UserId });
    }
}