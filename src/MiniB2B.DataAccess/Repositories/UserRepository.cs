using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MiniB2BDbContext _context;

    public UserRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return _context.Users.AnyAsync(u => u.Email == email);
    }

    public Task<bool> UserNameExistsAsync(string userName)
    {
        return _context.Users.AnyAsync(u => u.UserName == userName);
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
}