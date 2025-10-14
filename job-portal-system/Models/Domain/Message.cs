namespace job_portal_system.Models.Domain;

public class Message
{
    public int Id { get; set; }
    public string SenderId { get; set; } = "";
    public User Sender { get; set; } = default!;
    public string ReceiverId { get; set; } = "";
    public User Receiver { get; set; } = default!;
    public string Content { get; set; } = "";
    public DateTime SentAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; }
}