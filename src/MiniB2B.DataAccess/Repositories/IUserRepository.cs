using MiniB2B.Entities.Dtos.Auth;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Users;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> UserNameExistsAsync(string userName, int? excludeId = null);
    Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);
    Task AddAsync(User user);
    Task<bool> AnyInRoleAsync(UserRole role);

    Task<PagedResult<AdminUserListItemDto>> SearchAsync(AdminUserSearchRequest request);
    Task<UserEditDto?> GetEditAsync(int id);
    Task<User?> GetByIdAsync(int id);
    Task<UserAuthStateDto?> GetAuthStateAsync(int userId);
    Task SaveChangesAsync();
}