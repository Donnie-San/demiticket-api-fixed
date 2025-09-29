using System.ComponentModel.DataAnnotations;

namespace DemiTicket.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "User";
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; } = string.Empty;
        public bool IsRequestingAuthorization { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
