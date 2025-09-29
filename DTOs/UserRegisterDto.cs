using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Password { get; set; } = string.Empty;
    }
}
