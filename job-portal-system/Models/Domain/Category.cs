namespace job_portal_system.Models.Domain;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}