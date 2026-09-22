using System.ComponentModel.DataAnnotations;

namespace MiniB2B.Entities.Dtos.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Kullanıcı adı veya e-posta zorunludur.")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}