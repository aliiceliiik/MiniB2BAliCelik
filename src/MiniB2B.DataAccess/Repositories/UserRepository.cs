using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.DataAccess.Extensions;
using MiniB2B.Entities.Dtos.Auth;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Users;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MiniB2BDbContext _context;

    public UserRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Users.Where(u => u.Email == email);

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return query.AnyAsync();
    }

    public Task<bool> UserNameExistsAsync(string userName, int? excludeId = null)
    {
        var query = _context.Users.Where(u => u.UserName == userName);

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return query.AnyAsync();
    }

    public Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail
                                   || u.Email == userNameOrEmail);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    public Task<bool> AnyInRoleAsync(UserRole role)
    {
        return _context.Users.AnyAsync(u => u.Role == role);
    }

    public Task<PagedResult<AdminUserListItemDto>> SearchAsync(AdminUserSearchRequest request)
    {
        IQueryable<User> query = _context.Users;

        if (request.Role.HasValue)
            query = query.Where(u => u.Role == request.Role.Value);

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm;
            query = query.Where(u =>
                (u.FirstName + " " + u.LastName).Contains(term) ||
                u.UserName.Contains(term) ||
                u.Email.Contains(term) ||
                u.Phone.Contains(term));
        }

        return query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new AdminUserListItemDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
                UserName = u.UserName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                OrderCount = u.Orders.Count
            })
            .ToPagedResultAsync(request.Page, request.PageSize);
    }

    public Task<UserEditDto?> GetEditAsync(int id)
    {
        return _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserEditDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserName = u.UserName,
                Phone = u.Phone,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                OrderCount = u.Orders.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public Task<UserAuthStateDto?> GetAuthStateAsync(int userId)
    {
        return _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserAuthStateDto { IsActive = u.IsActive, Role = u.Role })
            .FirstOrDefaultAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}