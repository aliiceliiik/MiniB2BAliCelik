using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Users;

namespace MiniB2B.Business.Services;

public interface IAdminUserService
{
    Task<PagedResult<AdminUserListItemDto>> SearchAsync(AdminUserSearchRequest request);
    Task<UserEditDto?> GetForEditAsync(int id);
    Task<ServiceResult> UpdateAsync(UserEditDto form, int currentAdminId);
}