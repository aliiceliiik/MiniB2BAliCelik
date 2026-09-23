using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Auth;

public class UserAuthStateDto
{
    public bool IsActive { get; init; }
    public UserRole Role { get; init; }
}