using DemiTicket.Data;
using DemiTicket.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemiTicket.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/users")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminUserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] AdminUserQueryDto query)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(query.Search))
                usersQuery = usersQuery.Where(u => u.Email.Contains(query.Search) || u.Username.Contains(query.Search));

            if (!string.IsNullOrEmpty(query.Role))
                usersQuery = usersQuery.Where(u => u.Role == query.Role);

            if (query.IsRequestingAuthorization.HasValue)
                usersQuery = usersQuery.Where(u => u.IsRequestingAuthorization == query.IsRequestingAuthorization);

            if (query.IsDeleted.HasValue)
                usersQuery = usersQuery.Where(u => u.IsDeleted == query.IsDeleted);

            if (!string.IsNullOrEmpty(query.SortBy)) {
                usersQuery = query.SortOrder == "desc"
                    ? usersQuery.OrderByDescending(u => EF.Property<object>(u, query.SortBy))
                    : usersQuery.OrderBy(u => EF.Property<object>(u, query.SortBy));
            }

            var totalItems = await usersQuery.CountAsync();
            var items = await usersQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => new AdminUserListItemDto {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    IsRequestingAuthorization = u.IsRequestingAuthorization,
                    IsDeleted = u.IsDeleted
                })
                .ToListAsync();

            var result = new PaginatedResult<AdminUserListItemDto> {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetail([FromRoute] Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found");

            var dto = new AdminUserDetailDto {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsRequestingAuthorization = user.IsRequestingAuthorization,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                IsDeleted = user.IsDeleted
            };

            return Ok(dto);
        }

    }
}
