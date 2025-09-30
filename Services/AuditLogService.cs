using DemiTicket.Data;
using DemiTicket.Models;

namespace DemiTicket.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(Guid? userId, string action, string endpoint, string? metadata = null)
        {
            var log = new AuditLog {
                UserId = userId,
                Action = action,
                Endpoint = endpoint,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}