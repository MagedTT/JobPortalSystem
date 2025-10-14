using Microsoft.AspNetCore.Identity;

namespace job_portal_system.Models.Domain
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = true; // so that if we delete the user we just mark (IsActive = false) and prevent حوارات كتير
        public Employer Employer { get; set; } = default!;
        public JobSeeker JobSeeker { get; set; } = default!;
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
