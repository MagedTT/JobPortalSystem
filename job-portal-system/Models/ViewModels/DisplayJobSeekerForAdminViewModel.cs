namespace job_portal_system.Models.ViewModels;

public class DisplayJobSeekerForAdminViewModel
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Education { get; set; } = "";
    public int ExperienceYears { get; set; }
    public bool IsActive { get; set; } = true;
    public int TotalPages { get; set; }
}