using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class UserChangeAuthorizationRequestDto
    {
        [Required]
        public bool IsRequestingAuthorization { get; set; }
    }
}
