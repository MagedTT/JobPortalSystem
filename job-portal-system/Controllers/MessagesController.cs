using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace job_portal_system.Controllers
{
    [Authorize(Roles = "JobSeeker,Employer")]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Messages/Inbox
        public async Task<IActionResult> Inbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var viewModels = new List<MessageViewModel>();

            foreach (var message in messages)
            {
                var sender = await _userManager.FindByIdAsync(message.SenderId);
                
                // Try to get sender's display name (JobSeeker or Employer)
                var senderJobSeeker = await _context.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == message.SenderId);
                var senderEmployer = await _context.Employers
                    .FirstOrDefaultAsync(e => e.UserId == message.SenderId);

                string senderName = senderEmployer?.CompanyName 
                    ?? (senderJobSeeker != null ? $"{senderJobSeeker.FirstName} {senderJobSeeker.LastName}" : sender?.UserName) 
                    ?? "Unknown";

                // Extract subject from content if it starts with "Subject:"
                string subject = "No Subject";
                string content = message.Content;
                
                if (message.Content.StartsWith("Subject: "))
                {
                    var lines = message.Content.Split('\n', 2);
                    if (lines.Length > 0)
                    {
                        subject = lines[0].Replace("Subject: ", "").Trim();
                        content = lines.Length > 1 ? lines[1].Trim() : "";
                    }
                }

                // Get current user's display name
                var currentJobSeeker = await _context.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == userId);
                var currentEmployer = await _context.Employers
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                string receiverName = currentEmployer?.CompanyName 
                    ?? (currentJobSeeker != null ? $"{currentJobSeeker.FirstName} {currentJobSeeker.LastName}" : User.Identity?.Name) 
                    ?? "";

                viewModels.Add(new MessageViewModel
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderName = senderName,
                    ReceiverId = message.ReceiverId,
                    ReceiverName = receiverName,
                    Subject = subject,
                    Content = content,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead,
                    IsSentByMe = false
                });
            }

            return View(viewModels);
        }

        // GET: Messages/Sent
        public async Task<IActionResult> Sent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var viewModels = new List<MessageViewModel>();

            foreach (var message in messages)
            {
                var receiver = await _userManager.FindByIdAsync(message.ReceiverId);
                
                // Try to get receiver's display name (JobSeeker or Employer)
                var receiverJobSeeker = await _context.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == message.ReceiverId);
                var receiverEmployer = await _context.Employers
                    .FirstOrDefaultAsync(e => e.UserId == message.ReceiverId);

                string receiverName = receiverEmployer?.CompanyName 
                    ?? (receiverJobSeeker != null ? $"{receiverJobSeeker.FirstName} {receiverJobSeeker.LastName}" : receiver?.UserName) 
                    ?? "Unknown";

                // Extract subject from content if it starts with "Subject:"
                string subject = "No Subject";
                string content = message.Content;
                
                if (message.Content.StartsWith("Subject: "))
                {
                    var lines = message.Content.Split('\n', 2);
                    if (lines.Length > 0)
                    {
                        subject = lines[0].Replace("Subject: ", "").Trim();
                        content = lines.Length > 1 ? lines[1].Trim() : "";
                    }
                }

                // Get current user's display name
                var currentJobSeeker = await _context.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == userId);
                var currentEmployer = await _context.Employers
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                string senderName = currentEmployer?.CompanyName 
                    ?? (currentJobSeeker != null ? $"{currentJobSeeker.FirstName} {currentJobSeeker.LastName}" : User.Identity?.Name) 
                    ?? "";

                viewModels.Add(new MessageViewModel
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderName = senderName,
                    ReceiverId = message.ReceiverId,
                    ReceiverName = receiverName,
                    Subject = subject,
                    Content = content,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead,
                    IsSentByMe = true
                });
            }

            return View(viewModels);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int id, bool isSent = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && 
                    (m.SenderId == userId || m.ReceiverId == userId));

            if (message == null)
            {
                TempData["Error"] = "Message not found.";
                return RedirectToAction(nameof(Inbox));
            }

            // Mark as read if it's a received message
            if (message.ReceiverId == userId && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            var isSentByMe = message.SenderId == userId;
            var otherUserId = isSentByMe ? message.ReceiverId : message.SenderId;
            var otherUser = await _userManager.FindByIdAsync(otherUserId);

            // Try to get other user's display name (JobSeeker or Employer)
            var otherJobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == otherUserId);
            var otherEmployer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserId == otherUserId);

            string otherUserName = otherEmployer?.CompanyName 
                ?? (otherJobSeeker != null ? $"{otherJobSeeker.FirstName} {otherJobSeeker.LastName}" : otherUser?.UserName) 
                ?? "Unknown";

            // Get current user's display name
            var currentJobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);
            var currentEmployer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserId == userId);

            string currentUserName = currentEmployer?.CompanyName 
                ?? (currentJobSeeker != null ? $"{currentJobSeeker.FirstName} {currentJobSeeker.LastName}" : User.Identity?.Name) 
                ?? "";

            // Extract subject from content if it starts with "Subject:"
            string subject = "No Subject";
            string content = message.Content;
            
            if (message.Content.StartsWith("Subject: "))
            {
                var lines = message.Content.Split('\n', 2);
                if (lines.Length > 0)
                {
                    subject = lines[0].Replace("Subject: ", "").Trim();
                    content = lines.Length > 1 ? lines[1].Trim() : "";
                }
            }

            var viewModel = new MessageViewModel
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = isSentByMe ? currentUserName : otherUserName,
                ReceiverId = message.ReceiverId,
                ReceiverName = isSentByMe ? otherUserName : currentUserName,
                Subject = subject,
                Content = content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                IsSentByMe = isSentByMe
            };

            return View(viewModel);
        }

        // POST: Messages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool isSent = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && 
                    (m.SenderId == userId || m.ReceiverId == userId));

            if (message == null)
            {
                TempData["Error"] = "Message not found.";
                return RedirectToAction(nameof(Inbox));
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message deleted successfully.";
            
            return isSent ? RedirectToAction(nameof(Sent)) : RedirectToAction(nameof(Inbox));
        }

        // POST: Messages/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && m.ReceiverId == userId);

            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Inbox));
        }

        // GET: Messages/Reply/5
        public async Task<IActionResult> Reply(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var originalMessage = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && 
                    (m.SenderId == userId || m.ReceiverId == userId));

            if (originalMessage == null)
            {
                TempData["Error"] = "Message not found.";
                return RedirectToAction(nameof(Inbox));
            }

            // Determine who to reply to (the other person in the conversation)
            var replyToUserId = originalMessage.SenderId == userId 
                ? originalMessage.ReceiverId 
                : originalMessage.SenderId;

            var replyToUser = await _userManager.FindByIdAsync(replyToUserId);

            // Try to get reply-to user's display name (JobSeeker or Employer)
            var replyToJobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == replyToUserId);
            var replyToEmployer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserId == replyToUserId);

            string replyToUserName = replyToEmployer?.CompanyName 
                ?? (replyToJobSeeker != null ? $"{replyToJobSeeker.FirstName} {replyToJobSeeker.LastName}" : replyToUser?.UserName) 
                ?? "Unknown";

            // Extract original subject
            string originalSubject = "No Subject";
            if (originalMessage.Content.StartsWith("Subject: "))
            {
                var lines = originalMessage.Content.Split('\n', 2);
                if (lines.Length > 0)
                {
                    originalSubject = lines[0].Replace("Subject: ", "").Trim();
                }
            }

            // Prepare reply subject (add "Re: " if not already present)
            string replySubject = originalSubject.StartsWith("Re: ", StringComparison.OrdinalIgnoreCase)
                ? originalSubject
                : $"Re: {originalSubject}";

            var viewModel = new SendMessageViewModel
            {
                SenderId = userId,
                ReceiverId = replyToUserId,
                ReceiverName = replyToUserName,
                Subject = replySubject,
                Content = "" // Empty for user to type their reply
            };

            ViewBag.OriginalMessage = originalMessage;
            ViewBag.OriginalSubject = originalSubject;

            return View(viewModel);
        }

        // POST: Messages/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(SendMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload receiver name if validation fails
                var receiver = await _userManager.FindByIdAsync(model.ReceiverId);
                var receiverJobSeeker = await _context.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == model.ReceiverId);
                var receiverEmployer = await _context.Employers
                    .FirstOrDefaultAsync(e => e.UserId == model.ReceiverId);
                model.ReceiverName = receiverEmployer?.CompanyName 
                    ?? (receiverJobSeeker != null ? $"{receiverJobSeeker.FirstName} {receiverJobSeeker.LastName}" : receiver?.UserName) 
                    ?? "Unknown";
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Format the message content with subject
            var messageContent = $"Subject: {model.Subject}\n{model.Content}";

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = model.ReceiverId,
                Content = messageContent,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reply sent successfully!";
            return RedirectToAction(nameof(Sent));
        }
    }
}
