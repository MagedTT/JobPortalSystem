using job_portal_system.Models.Domain;

namespace job_portal_system.Models.ViewModels;

public class JobSeekerProfileViewModel
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? ImagePath { get; set; }
    public string? CVPath { get; set; }
    public string Education { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string Location { get; set; } = "";
    public List<string>? Skills { get; set; }
}