namespace job_portal_system.Models.ViewModels;

public class FavoriteJobViewModel
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = "";
    public string JobDescription { get; set; } = "";
    public int JobYearsOfExperience { get; set; }
    public int JobSalary { get; set; }
    public string JobLocation { get; set; } = "";
    public int JobCategoryId { get; set; }
    public string JobCategoryName { get; set; } = "";
    public bool JobIsActive { get; set; }
}