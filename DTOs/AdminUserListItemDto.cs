namespace DemiTicket.DTOs
{
    public class AdminUserListItemDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsRequestingAuthorization { get; set; }
        public bool IsDeleted { get; set; }
    }
}
