using System.ComponentModel.DataAnnotations;

namespace job_portal_system.Models.ViewModels
{
    public class EmployerProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Company Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Company Logo")]
        public string? LogoPath { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        [Display(Name = "Company Website")]
        public string? Website { get; set; }

        [Display(Name = "Industry")]
        public string? Industry { get; set; }

        public bool IsApproved { get; set; }
        public int TotalJobs { get; set; }

        // File upload property
        [Display(Name = "Upload Company Logo")]
        public IFormFile? LogoFile { get; set; }
    }
}
