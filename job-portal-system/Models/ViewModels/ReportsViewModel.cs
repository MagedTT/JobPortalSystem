namespace job_portal_system.Models.ViewModels
{
    public class ReportsViewModel
    {
        public List<UserReportDto> UserReports { get; set; } = new();
        public List<JobReportDto> JobReports { get; set; } = new();
        public List<ApplicationReportDto> ApplicationReports { get; set; } = new();
        public List<EmployerReportDto> EmployerReports { get; set; } = new();
        public List<JobSeekerReportDto> JobSeekerReports { get; set; } = new();
    }

    public class UserReportDto
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class JobReportDto
    {
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string JobType { get; set; } = string.Empty;
        public int ApplicationsCount { get; set; }
        public DateTime PostedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class ApplicationReportDto
    {
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; }
        public int ExperienceYears { get; set; }
    }

    public class EmployerReportDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public DateTime RegisteredDate { get; set; }
    }

    public class JobSeekerReportDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int TotalApplications { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
