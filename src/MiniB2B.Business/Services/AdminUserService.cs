using Microsoft.AspNetCore.Identity;
using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Users;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business.Services;

public class AdminUserService : IAdminUserService
{
    private const int MaxPageSize = 100;

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AdminUserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task<PagedResult<AdminUserListItemDto>> SearchAsync(AdminUserSearchRequest request)
    {
        request.Page = Math.Max(1, request.Page);
        request.PageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        request.SearchTerm = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : request.SearchTerm.Trim();

        return _userRepository.SearchAsync(request);
    }

    public Task<UserEditDto?> GetForEditAsync(int id) => _userRepository.GetEditAsync(id);

    public async Task<ServiceResult> UpdateAsync(UserEditDto form, int currentAdminId)
    {
        var user = await _userRepository.GetByIdAsync(form.Id);
        if (user is null)
            return ServiceResult.Failure("Kullanıcı bulunamadı.");

        if (!Enum.IsDefined(form.Role))
            return ServiceResult.Failure("Geçersiz rol.");

        if (form.Id == currentAdminId && (form.Role != user.Role || !form.IsActive))
            return ServiceResult.Failure("Kendi rolünüzü değiştiremez veya kendi hesabınızı pasife alamazsınız.");

        var email = form.Email.Trim();
        var userName = form.UserName.Trim();

        if (await _userRepository.EmailExistsAsync(email, form.Id))
            return ServiceResult.Failure("Bu e-posta adresi başka bir kullanıcıya ait.");

        if (await _userRepository.UserNameExistsAsync(userName, form.Id))
            return ServiceResult.Failure("Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.");

        user.FirstName = form.FirstName.Trim();
        user.LastName = form.LastName.Trim();
        user.Email = email;
        user.UserName = userName;
        user.Phone = form.Phone.Trim();
        user.Role = form.Role;
        user.IsActive = form.IsActive;

        if (!string.IsNullOrWhiteSpace(form.NewPassword))
            user.PasswordHash = _passwordHasher.HashPassword(user, form.NewPassword);

        await _userRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
}