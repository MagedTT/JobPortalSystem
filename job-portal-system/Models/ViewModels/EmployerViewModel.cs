namespace job_portal_system.Models.ViewModels;

public class EmployerViewModel
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string Description { get; set; } = "";
    public string? LogoPath { get; set; }
    public string Location { get; set; } = "";
    public bool IsApproved { get; set; }
}