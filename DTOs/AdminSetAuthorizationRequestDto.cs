using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class AdminSetAuthorizationRequestDto
    {
        [Required]
        public bool NewAuthorizationRequest { get; set; }
    }
}
