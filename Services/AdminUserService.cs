using DemiTicket.Data;
using DemiTicket.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DemiTicket.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly AppDbContext _context;

        public AdminUserService(AppDbContext context) => _context = context;

        public async Task<PaginatedResult<AdminUserListItemDto>> GetUsers(AdminUserQueryDto query)
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

            return new PaginatedResult<AdminUserListItemDto> {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
            };
        }

        public async Task<AdminUserDetailDto> GetUserDetail(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new Exception("User not found");

            return new AdminUserDetailDto {
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
        }

        public async Task SetRole(Guid id, string newRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            user.Role = newRole;
            await _context.SaveChangesAsync();
        }

        public async Task SetAuthorizationRequest(Guid id, bool isRequestingAuthorization)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            user.IsRequestingAuthorization = isRequestingAuthorization;
            await _context.SaveChangesAsync();
        }

        public async Task SetDeleted(Guid id, bool isDelete)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            user.IsDeleted = isDelete;
            await _context.SaveChangesAsync();
        }

        public async Task HardDelete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
