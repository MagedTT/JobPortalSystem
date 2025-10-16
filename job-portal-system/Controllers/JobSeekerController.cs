using System.Threading.Tasks;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers;

public class JobSeekerController : Controller
{
    private readonly IJobSeekerService _jobSeekerService;
    private readonly IWebHostEnvironment _env;

    public JobSeekerController(IJobSeekerService jobSeekerService, IWebHostEnvironment env)
    {
        _jobSeekerService = jobSeekerService;
        _env = env;
    }
    public async Task<IActionResult> Index(string id)
    {
        var jobSeeker = await _jobSeekerService.GetJobSeekerProfileAsync(id);

        if (jobSeeker is null)
            return NotFound();

        JobSeekerProfileViewModel model = new()
        {
            Id = jobSeeker.Id,
            UserId = jobSeeker.UserId,
            FirstName = jobSeeker.FirstName,
            LastName = jobSeeker.LastName,
            ImagePath = jobSeeker.ImagePath,
            CVPath = jobSeeker.CVPath,
            Education = jobSeeker.Education,
            ExperienceYears = jobSeeker.ExperienceYears,
            Location = jobSeeker.Location,
            Skills = jobSeeker.Skills?.Split(',').ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> DownloadCV(string cvPath)
    {
        if (string.IsNullOrEmpty(cvPath))
            return NotFound();

        var fullCVPath = Path.Combine($"{_env.WebRootPath}/cvs", cvPath);

        if (!System.IO.File.Exists(fullCVPath))
            return NotFound();

        var fileInBytes = System.IO.File.ReadAllBytes(fullCVPath);
        var contentType = "application/pdf";

        return File(fileInBytes, contentType);
    }

    public async Task<IActionResult> EditJobSeekerProfile(string id)
    {
        var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(id);

        if (jobSeeker is null)
            return NotFound();

        EditJobSeekerViewModel model = new()
        {
            Id = jobSeeker.Id,
            UserId = jobSeeker.UserId,
            FirstName = jobSeeker.FirstName,
            LastName = jobSeeker.LastName,
            ImagePath = jobSeeker.ImagePath,
            CVPath = jobSeeker.CVPath,
            Education = jobSeeker.Education,
            ExperienceYears = jobSeeker.ExperienceYears,
            Location = jobSeeker.Location,
            Skills = jobSeeker.Skills
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditJobSeekerProfile(EditJobSeekerViewModel model)
    {
        var jobSeeker = await _jobSeekerService.GetJobSeekerProfileAsync(model.UserId);

        if (jobSeeker is null)
            return NotFound();

        await _jobSeekerService.EditJobSeekerProfileAsync(model, jobSeeker);

        return RedirectToAction(nameof(Index), new { id = model.UserId });
    }
}