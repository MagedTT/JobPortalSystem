using System.ComponentModel.DataAnnotations;

namespace job_portal_system.Models.ViewModels
{
    public class JobSeekerProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Education is required")]
        public string Education { get; set; } = string.Empty;

        [Required(ErrorMessage = "Experience years is required")]
        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
        [Display(Name = "Years of Experience")]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Skills (comma separated)")]
        public string? Skills { get; set; }

        [Display(Name = "Profile Image")]
        public string? ImagePath { get; set; }

        [Display(Name = "CV/Resume")]
        public string? CVPath { get; set; }

        public bool IsApproved { get; set; }

        // File upload properties
        [Display(Name = "Upload Profile Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Upload CV/Resume")]
        public IFormFile? CVFile { get; set; }
    }
}
