using BCrypt.Net;
using DemiTicket.Data;
using DemiTicket.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemiTicket.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new UserInfoDto {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsRequestingAuthorization = user.IsRequestingAuthorization
            });
        }

        [HttpPatch("me/change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] UserChangeUsernameDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            user.Username = dto.NewUsername;
            await _context.SaveChangesAsync();

            return Ok("Username has been updated.");
        }

        [HttpPatch("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                return Unauthorized("Old password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password has been updated.");
        }

        [HttpPatch("me/change-authorization-request")]
        public async Task<IActionResult> ChangeAuthorizationRequest([FromBody] UserChangeAuthorizationRequestDto dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            user.IsRequestingAuthorization = dto.IsRequestingAuthorization;
            await _context.SaveChangesAsync();

            return Ok("Authorization request status updated.");
        }
    }
}