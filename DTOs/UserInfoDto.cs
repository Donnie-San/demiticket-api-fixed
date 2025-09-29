namespace DemiTicket.DTOs
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsRequestingAuthorization { get; set; }
    }
}
