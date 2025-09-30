using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class AdminSetRoleDto
    {
        [Required]
        public string NewRole { get; set; }
    }
}
