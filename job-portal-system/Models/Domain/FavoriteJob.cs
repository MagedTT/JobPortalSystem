namespace job_portal_system.Models.Domain;

public class FavoriteJob
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = default!;
    public string JobSeekerId { get; set; } = "";
    public JobSeeker JobSeeker { get; set; } = default!;
    public DateTime SavedDate { get; set; } = DateTime.Now;
}