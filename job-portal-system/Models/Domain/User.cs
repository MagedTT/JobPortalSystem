using Microsoft.AspNetCore.Identity;

namespace job_portal_system.Models.Domain
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Employer / JobSeeker / Admin
    }
}
