using System.ComponentModel.DataAnnotations;

namespace job_portal_system.Models.ViewModels
{
    public class RegisterEmployerViewModel
    {
        [Required, Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Company Email")]
        public string CompanyEmail { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match"), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // optional: website or short info
        [Url, Display(Name = "Company Website")]
        public string? Website { get; set; }

        [Display(Name = "Industry")]
        public string? Industry { get; set; }
    }
}
