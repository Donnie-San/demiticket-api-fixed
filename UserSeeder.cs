using DemiTicket.Data;
using DemiTicket.Models;
using BCrypt.Net;

namespace DemiTicket.Seeding
{
    public static class UserSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any()) return;

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "alice",
                    Email = "alice@example.com",
                    Role = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    IsEmailVerified = true,
                    IsRequestingAuthorization = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    LastLogin = DateTime.UtcNow.AddDays(-1)
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "bob",
                    Email = "bob@example.com",
                    Role = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("secure456"),
                    IsEmailVerified = true,
                    IsRequestingAuthorization = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    LastLogin = DateTime.UtcNow.AddDays(-2)
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "charlie",
                    Email = "charlie@example.com",
                    Role = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("charlie789"),
                    IsEmailVerified = false,
                    IsRequestingAuthorization = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    LastLogin = null
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "daisy",
                    Email = "daisy@example.com",
                    Role = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("daisy321"),
                    IsEmailVerified = true,
                    IsRequestingAuthorization = false,
                    IsDeleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    LastLogin = DateTime.UtcNow.AddDays(-30)
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}