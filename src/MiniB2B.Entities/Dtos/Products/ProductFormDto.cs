using System.ComponentModel.DataAnnotations;

namespace MiniB2B.Entities.Dtos.Products;

public class ProductFormDto
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Kategori seçiniz.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Ürün kodu zorunludur.")]
    [StringLength(50, ErrorMessage = "Ürün kodu en fazla 50 karakter olabilir.")]
    public string ProductCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(100, ErrorMessage = "Marka en fazla 100 karakter olabilir.")]
    public string Brand { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Üretici kodu en fazla 50 karakter olabilir.")]
    public string? ManufacturerCode { get; set; }

    [StringLength(50, ErrorMessage = "Özel kod 1 en fazla 50 karakter olabilir.")]
    public string? SpecialCode1 { get; set; }

    [StringLength(50, ErrorMessage = "Özel kod 2 en fazla 50 karakter olabilir.")]
    public string? SpecialCode2 { get; set; }

    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz.")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kritik stok seviyesi negatif olamaz.")]
    public int CriticalStockLevel { get; set; } = 10;

    [Range(0.01, 999999999, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
}