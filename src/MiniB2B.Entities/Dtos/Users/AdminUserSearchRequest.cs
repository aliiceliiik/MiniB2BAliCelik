using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Users;

public class AdminUserSearchRequest
{
    public string? SearchTerm { get; set; }
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}