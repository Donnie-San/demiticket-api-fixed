using BCrypt.Net;
using DemiTicket.Data;
using DemiTicket.DTOs;
using DemiTicket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace DemiTicket.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IAdminUserService _adminUserService;
        private readonly IAuditLogService _auditLogService;

        public UserController(AppDbContext context, IUserService userService, IAdminUserService adminUserService, IAuditLogService auditLogService)
        {
            _context = context;
            _userService = userService;
            _adminUserService = adminUserService;
            _auditLogService = auditLogService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            var result = await _userService.GetUserInfo(userId);
            return Ok(result);
        }

        [HttpPatch("me/change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] UserChangeUsernameDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            await _userService.ChangeUsername(userId, dto.NewUsername);
            await _auditLogService.LogAsync(userId, "ChangeUserName", "api/users/me/change-username");
            return Ok("Username has been updated.");
        }

        [HttpPatch("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            await _userService.ChangePassword(userId, dto.OldPassword, dto.NewPassword);
            await _auditLogService.LogAsync(userId, "ChangePassword", "api/users/me/change-password");
            return Ok("Password has been updated.");
        }

        [HttpPatch("me/change-authorization-request")]
        public async Task<IActionResult> ChangeAuthorizationRequest([FromBody] UserChangeAuthorizationRequestDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            await _userService.ChangeAuthorizationRequest(userId, dto.IsRequestingAuthorization);
            await _auditLogService.LogAsync(userId, "ChangeAuthorizationRequest", "api/users/me/change-authorization-request");
            return Ok("Authorization request status updated.");
        }

        // ##### ADMIN ZONE #####

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery] AdminUserQueryDto query)
        {
            var result = await _adminUserService.GetUsers(query);
            return Ok(result);
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDetail(Guid id)
        {
            var result = await _adminUserService.GetUserDetail(id);
            return Ok(result);
        }

        [HttpPatch("admin/{id}/set-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetRole(Guid id, [FromBody] AdminSetRoleDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
                return Unauthorized("Invalid token");

            await _adminUserService.SetRole(id, dto.NewRole);
            await _auditLogService.LogAsync(adminId, "SetRole", $"api/users/admin/{id}/set-role", $"TargetUserId: {id}, NewRole: {dto.NewRole}");
            return NoContent();
        }

        [HttpPatch("admin/{id}/set-authorization-request")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetAuthorizationRequest(Guid id, [FromBody] AdminSetAuthorizationRequestDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
                return Unauthorized("Invalid token");

            await _adminUserService.SetAuthorizationRequest(id, dto.NewAuthorizationRequest);

            await _auditLogService.LogAsync(adminId, "SetAuthorizationRequest", $"api/users/admin/{id}/set-authorization-request",
                $"TargetUserId: {id}, NewAuthorizationRequest: {dto.NewAuthorizationRequest}");
            return NoContent();
        }

        [HttpPatch("admin/{id}/set-deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetDeleted(Guid id, [FromBody] AdminSetDeletedDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
                return Unauthorized("Invalid token");

            await _adminUserService.SetDeleted(id, dto.IsDeleted);

            await _auditLogService.LogAsync(adminId, "SetDeleted", $"api/users/admin/{id}/set-deleted", $"TargetUserId: {id}, NewIsDeleted: {dto.IsDeleted}");
            return NoContent();
        }

        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
                return Unauthorized("Invalid token");

            await _adminUserService.HardDelete(id);

            await _auditLogService.LogAsync(adminId, "HardDelete", $"api/users/admin/{id}", $"TargetUserId: {id}");
            return NoContent();
        }
    }
}
