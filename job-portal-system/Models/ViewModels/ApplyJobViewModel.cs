using System.ComponentModel.DataAnnotations;

namespace job_portal_system.Models.ViewModels
{
    public class ApplyJobViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobSeekerId { get; set; } = string.Empty;

        [Display(Name = "Cover Letter")]
        [StringLength(1000, ErrorMessage = "Cover letter cannot exceed 1000 characters.")]
        public string? CoverLetter { get; set; }
    }
}
