using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Auth;

public class AuthenticatedUserDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public UserRole Role { get; init; }
}