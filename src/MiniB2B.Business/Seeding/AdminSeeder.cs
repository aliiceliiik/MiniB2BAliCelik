using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business.Seeding;

public class AdminSeeder : IAdminSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AdminSeeder> _logger;

    public AdminSeeder(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ILogger<AdminSeeder> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(AdminSeedOptions options)
    {
        if (await _userRepository.AnyInRoleAsync(UserRole.Admin))
            return;

        if (string.IsNullOrWhiteSpace(options.Email) ||
            string.IsNullOrWhiteSpace(options.UserName) ||
            string.IsNullOrWhiteSpace(options.Password))
        {
            _logger.LogWarning("Sistemde admin yok ve AdminSeed ayarları eksik. Admin oluşturulmadı.");
            return;
        }

        var admin = new User
        {
            FirstName = options.FirstName,
            LastName = options.LastName,
            Email = options.Email,
            UserName = options.UserName,
            Phone = options.Phone,
            Role = UserRole.Admin
        };

        admin.PasswordHash = _passwordHasher.HashPassword(admin, options.Password);
        await _userRepository.AddAsync(admin);

        _logger.LogInformation("İlk admin kullanıcısı oluşturuldu: {UserName}", admin.UserName);
    }
}