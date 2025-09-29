using DemiTicket.Models;

namespace DemiTicket.Services
{
    public interface ITokenService
    {
        string GenerateJWT(User user);
        string GenerateRandomToken();
    }
}
