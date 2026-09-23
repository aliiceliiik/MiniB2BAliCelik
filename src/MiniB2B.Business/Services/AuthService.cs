using Microsoft.AspNetCore.Identity;
using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Auth;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Business.Common;

namespace MiniB2B.Business.Services;

public class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Kullanıcı adı/e-posta veya şifre hatalı.";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ServiceResult<AuthenticatedUserDto>> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim();
        var userName = request.UserName.Trim();

        if (await _userRepository.EmailExistsAsync(email))
            return ServiceResult<AuthenticatedUserDto>.Failure("Bu e-posta adresi zaten kayıtlı.");

        if (await _userRepository.UserNameExistsAsync(userName))
            return ServiceResult<AuthenticatedUserDto>.Failure("Bu kullanıcı adı zaten kullanılıyor.");

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            UserName = userName,
            Phone = request.Phone.Trim(),
            Role = UserRole.Customer
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        try
        {
            await _userRepository.AddAsync(user);
        }
        catch (DbUpdateException)
        {
            throw new BusinessException("Bu e-posta veya kullanıcı adı kayıt sırasında başka bir kullanıcı tarafından alındı. Lütfen tekrar deneyin.");
        }

        return ServiceResult<AuthenticatedUserDto>.Success(ToAuthenticatedUser(user));
    }

    public async Task<ServiceResult<AuthenticatedUserDto>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail.Trim());

        if (user is null)
            return ServiceResult<AuthenticatedUserDto>.Failure(InvalidCredentialsMessage);

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verification == PasswordVerificationResult.Failed)
            return ServiceResult<AuthenticatedUserDto>.Failure(InvalidCredentialsMessage);

        if (!user.IsActive)
            return ServiceResult<AuthenticatedUserDto>.Failure("Hesabınız pasif durumdadır. Lütfen yönetici ile iletişime geçin.");

        return ServiceResult<AuthenticatedUserDto>.Success(ToAuthenticatedUser(user));
    }
    public async Task<bool> IsSessionValidAsync(int userId, string role)
    {
        var state = await _userRepository.GetAuthStateAsync(userId);

        return state is not null
            && state.IsActive
            && state.Role.ToString() == role;
    }

    private static AuthenticatedUserDto ToAuthenticatedUser(User user) => new()
    {
        Id = user.Id,
        FullName = $"{user.FirstName} {user.LastName}",
        UserName = user.UserName,
        Email = user.Email,
        Role = user.Role
    };
}