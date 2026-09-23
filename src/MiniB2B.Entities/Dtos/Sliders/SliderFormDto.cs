using System.ComponentModel.DataAnnotations;

namespace MiniB2B.Entities.Dtos.Sliders;

public class SliderFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(150, ErrorMessage = "Başlık en fazla 150 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [StringLength(500, ErrorMessage = "Bağlantı adresi en fazla 500 karakter olabilir.")]
    public string? LinkUrl { get; set; }

    [Range(0, 999, ErrorMessage = "Sıra 0 ile 999 arasında olmalıdır.")]
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}