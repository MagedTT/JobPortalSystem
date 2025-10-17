using job_portal_system.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Controllers;

[Authorize(Roles = "Admin")]
public class AdminContactController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminContactController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: AdminContact/Messages
    [HttpGet]
    public async Task<IActionResult> Messages()
    {
        var messages = await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return View(messages);
    }

    // GET: AdminContact/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);

        if (message == null)
        {
            TempData["Error"] = "Message not found.";
            return RedirectToAction(nameof(Messages));
        }

        // Mark as read
        if (!message.IsRead)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return View(message);
    }

    // POST: AdminContact/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);

        if (message == null)
        {
            TempData["Error"] = "Message not found.";
            return RedirectToAction(nameof(Messages));
        }

        _context.ContactMessages.Remove(message);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Message deleted successfully.";
        return RedirectToAction(nameof(Messages));
    }

    // POST: AdminContact/MarkAsRead/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);

        if (message == null)
        {
            return Json(new { success = false });
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}
