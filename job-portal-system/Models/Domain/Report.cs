namespace job_portal_system.Models.Domain;

// Needs Revision
public class Report
{
    public int Id { get; set; }
    public string Format { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string GeneratedByWithUserId { get; set; } = "";
    public string UserId { get; set; } = "";
    public User User { get; set; } = default!;
}