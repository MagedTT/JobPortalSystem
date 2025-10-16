using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using job_portal_system.Models;
using job_portal_system.Data;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("ToggleActivation/{userId}")]
    public IActionResult ToggleJobSeekerActivation(string userId)
    {
        var jobSeeker = _context.JobSeekers.Include(x => x.User).FirstOrDefault(x => x.UserId == userId);
        jobSeeker!.User.IsActive = !jobSeeker.User.IsActive;
        _context.SaveChanges();
        return Ok(new { isActive = jobSeeker.User.IsActive });
    }
}
