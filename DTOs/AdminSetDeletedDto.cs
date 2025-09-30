using System.ComponentModel.DataAnnotations;

namespace DemiTicket.DTOs
{
    public class AdminSetDeletedDto
    {
        [Required]
        public bool IsDeleted { get; set; }
    }
}
