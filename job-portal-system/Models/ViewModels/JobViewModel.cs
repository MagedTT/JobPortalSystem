namespace job_portal_system.Models.ViewModels
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Location { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string JobType { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string EmployerId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool HasApplied { get; set; } = false;
        public int ApplicationsCount { get; set; } = 0;
    }
}
