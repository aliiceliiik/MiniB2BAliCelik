namespace MiniB2B.Entities.Models;

public class Product : BaseEntity
{
    public int CategoryId { get; set; }
    public string ProductCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Brand { get; set; } = null!;
    public string? ManufacturerCode { get; set; }
    public string? SpecialCode1 { get; set; }
    public string? SpecialCode2 { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public int CriticalStockLevel { get; set; } = 10;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
}