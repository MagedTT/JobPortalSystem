namespace job_portal_system.Models.Domain;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();
}