namespace job_portal_system.Models.DTOs
{
    public class RegisterEmployerDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Industry { get; set; }
    }
}
