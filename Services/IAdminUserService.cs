using DemiTicket.DTOs;

namespace DemiTicket.Services
{
    public interface IAdminUserService
    {
        Task<PaginatedResult<AdminUserListItemDto>> GetUsers(AdminUserQueryDto query);
        Task<AdminUserDetailDto> GetUserDetail(Guid id);
        Task SetRole(Guid id, string newRole);
        Task SetAuthorizationRequest(Guid id, bool isRequestingAuthorization);
        Task SetDeleted(Guid id, bool isDelete);
        Task HardDelete(Guid id);
    }
}
