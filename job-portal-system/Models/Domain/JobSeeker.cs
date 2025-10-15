namespace job_portal_system.Models.Domain;

public class JobSeeker
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public User User { get; set; } = default!;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? ImagePath { get; set; }
    public string? CVPath { get; set; }
    public string Education { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string Location { get; set; } = "";
    public string? Skills { get; set; }
    public ICollection<FavoriteJob> FavoriteJobs { get; set; } = new List<FavoriteJob>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    // public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();
}