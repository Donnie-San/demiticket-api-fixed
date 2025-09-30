using DemiTicket.Data;
using DemiTicket.DTOs;
using DemiTicket.Models;
using DemiTicket.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DemiTicket.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;

        public AuthController(AppDbContext context, ITokenService tokenService, IEmailService emailService, IAuditLogService auditLogService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _auditLogService = auditLogService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already registered");

            var verifyToken = _tokenService.GenerateRandomToken();
            var newUser = new User {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                IsEmailVerified = false,
                EmailVerificationToken = verifyToken,
                IsRequestingAuthorization = false,
                CreatedAt = DateTime.UtcNow,
                LastLogin = null,
                IsDeleted = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var encodedToken = Uri.EscapeDataString(verifyToken);
            var verificationLink = $"https://localhost/verify-email?token={encodedToken}";
            var body = $"Please verify your email by clicking this link: <a href=\"{verificationLink}\">{verificationLink}</a>";
            await _emailService.SendEmailAsync(newUser.Email, "Verify your email", body);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            await _auditLogService.LogAsync(user.Id, "Register", "api/auth/register");
            return Ok("User registed. Please check your email!");
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var decodedToken = token;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == decodedToken);
            if (user == null) return BadRequest("Invalid token");

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(user.Id, "Register", "api/auth/verify-email");
            return Ok("Email verified successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) {
                await _auditLogService.LogAsync(null, "FailedLogin", "api/auth/login", $"Email: {dto.Email}");
                return Unauthorized("Invalid credentials!");
            }

            if (!user.IsEmailVerified)
                return Unauthorized("Please verify your email before logging in.");

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var accessToken = _tokenService.GenerateJWT(user);
            var refreshToken = _tokenService.GenerateRandomToken();

            var newRefreshToken = new RefreshToken {
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
            });

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _auditLogService.LogAsync(user.Id, "Login", "api/auth/login", $"IP: {ip}");
            return Ok(new {
                accessToken,
                role = user.Role
            });
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Missing refresh token");

            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (tokenEntity == null || !tokenEntity.IsActive)
                return Unauthorized("Invalid or expired refresh token");

            var user = await _context.Users.FindAsync(tokenEntity.UserId);
            if (user == null)
                return Unauthorized("User not found");

            var newAccessToken = _tokenService.GenerateJWT(user);
            var newRefreshToken = _tokenService.GenerateRandomToken();

            tokenEntity.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Add(new Models.RefreshToken {
                Token = newRefreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
            });
            await _context.SaveChangesAsync();

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
            });

            await _auditLogService.LogAsync(user.Id, "RefreshToken", "api/auth/refresh");
            return Ok(new {
                newAccessToken,
                role = user.Role
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token found");

            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized("Invalid token");

            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken && t.UserId == userId);

            if (tokenEntity == null || !tokenEntity.IsActive)
                return Unauthorized("Invalid or expired refresh token");

            tokenEntity.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            Response.Cookies.Delete("refresh_token");

            await _auditLogService.LogAsync(userId, "Logout", "api/auth/logout");
            return Ok("Logout successful");
        }
    }
}
