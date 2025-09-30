using DemiTicket.DTOs;

namespace DemiTicket.Services
{
    public interface IUserService
    {
        Task<UserInfoDto> GetUserInfo(Guid userId);
        Task ChangeUsername(Guid userId, string newUsername);
        Task ChangePassword(Guid userId, string oldPassword, string newPassword);
        Task ChangeAuthorizationRequest(Guid userId, bool isRequestingAuthorization);
    }
}
