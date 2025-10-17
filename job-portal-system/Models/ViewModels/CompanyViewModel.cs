namespace job_portal_system.Models.ViewModels
{
    public class CompanyViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public string? CompanySize { get; set; }
        public int ActiveJobsCount { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class CompanyDetailsViewModel : CompanyViewModel
    {
        public List<JobViewModel> Jobs { get; set; } = new List<JobViewModel>();
    }
}
