using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UserNameExistsAsync(string userName);
    Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);
    Task AddAsync(User user);
    Task<bool> AnyInRoleAsync(UserRole role);
}