using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class UserChangePasswordDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
