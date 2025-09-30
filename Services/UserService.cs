using DemiTicket.Data;
using DemiTicket.DTOs;
using Org.BouncyCastle.Bcpg;

namespace DemiTicket.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context) => _context = context;

        public async Task<UserInfoDto> GetUserInfo(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return new UserInfoDto {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsRequestingAuthorization = user.IsRequestingAuthorization
            };
        }

        public async Task ChangeUsername(Guid userId, string newUsername)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.Username = newUsername;
            await _context.SaveChangesAsync();
        }

        public async Task ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Old password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeAuthorizationRequest(Guid userId, bool isRequestingAuthorization)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.IsRequestingAuthorization = isRequestingAuthorization;
            await _context.SaveChangesAsync();
        }
    }
}
