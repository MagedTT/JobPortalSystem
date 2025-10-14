namespace job_portal_system.Models.Domain;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public User User { get; set; } = default!;
    public string Message { get; set; } = "";
    // public MessageType MessageType { get; set; } // Later
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}