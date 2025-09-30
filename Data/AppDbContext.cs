using DemiTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace DemiTicket.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected AppDbContext()
        {
        }
    }
}
