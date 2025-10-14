namespace job_portal_system.Models.Domain;

public class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = default!;
    public string JobSeekerId { get; set; } = "";
    public JobSeeker JobSeeker { get; set; } = default!;
    public string CoverLetter { get; set; } = "";
    public Status Status { get; set; } = Status.Pending;
    public DateTime ApplicationDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CVPath { get; set; } = "";
}