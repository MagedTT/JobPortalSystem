namespace job_portal_system.Models.ViewModels
{
    public class ApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CoverLetter { get; set; }
        public string? CVPath { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string JobType { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? JobDescription { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
