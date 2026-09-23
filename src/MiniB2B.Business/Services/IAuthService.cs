using MiniB2B.Entities.Dtos.Auth;
using MiniB2B.Entities.Dtos.Common;

namespace MiniB2B.Business.Services;

public interface IAuthService
{
    Task<ServiceResult<AuthenticatedUserDto>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthenticatedUserDto>> LoginAsync(LoginRequest request);
    Task<bool> IsSessionValidAsync(int userId, string role);
}