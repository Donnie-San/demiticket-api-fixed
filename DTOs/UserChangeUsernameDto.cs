using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class UserChangeUsernameDto
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string NewUsername { get; set; } = string.Empty;
    }
}
