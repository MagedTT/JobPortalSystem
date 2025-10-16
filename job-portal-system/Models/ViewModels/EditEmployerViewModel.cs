namespace job_portal_system.Models.ViewModels;

public class EditEmployerViewModel
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string Description { get; set; } = "";
    public string? LogoPath { get; set; }
    public IFormFile? LogoFile { get; set; }
    public string Location { get; set; } = "";
}