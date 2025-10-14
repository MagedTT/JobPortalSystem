namespace job_portal_system.Models.Domain;

public class JobRequirement
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = default!;
    public string Name { get; set; } = "";
}