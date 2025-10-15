namespace job_portal_system.Models.Domain;

public class Employer
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public User User { get; set; } = default!;
    public string CompanyName { get; set; } = "";
    public string Description { get; set; } = "";
    public string? LogoPath { get; set; }
    public string Location { get; set; } = "";
    public bool IsApproved { get; set; } = false;
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}