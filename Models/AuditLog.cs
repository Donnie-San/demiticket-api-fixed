namespace DemiTicket.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public string Action { get; set; } = default!;
        public string? Endpoint { get; set; }
        public string? Metadata { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
