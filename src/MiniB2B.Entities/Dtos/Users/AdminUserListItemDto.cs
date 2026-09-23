using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Users;

public class AdminUserListItemDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public int OrderCount { get; init; }
}