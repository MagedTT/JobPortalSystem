namespace job_portal_system.Models.Domain
{
    public class Job
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EmployerId { get; set; } = "";
        public Employer Employer { get; set; } = default!;
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int YearsOfExperience { get; set; } // was (string ExperienceLevel) but I made it (int YearsOfExperience) To ease the searching process later since JobSeeker has (int ExperienceYears).
        public int Salary { get; set; }
        public string Location { get; set; } = "";
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public JobType JobType { get; set; } = JobType.FullTime;
        public DateTime PostedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public ICollection<FavoriteJob> FavoriteJobs { get; set; } = new List<FavoriteJob>();
        public ICollection<JobRequirement> JobRequirements { get; set; } = new List<JobRequirement>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
