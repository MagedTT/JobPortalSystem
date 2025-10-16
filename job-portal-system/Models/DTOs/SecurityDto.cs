namespace job_portal_system.Models.DTOs
{
    public class SecurityDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
