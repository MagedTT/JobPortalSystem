using System.ComponentModel.DataAnnotations;

namespace job_portal_system.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
