namespace DemiTicket.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(Guid? userId, string action, string endpoint, string? metadata = null);
    }
}
