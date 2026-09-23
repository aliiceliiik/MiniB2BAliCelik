using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Products;

public class ProductDetailDto
{
    public int Id { get; init; }
    public string ProductCode { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Brand { get; init; } = null!;
    public string? ManufacturerCode { get; init; }
    public string? SpecialCode1 { get; init; }
    public string? SpecialCode2 { get; init; }
    public string? ImageUrl { get; init; }
    public string CategoryName { get; init; } = null!;
    public decimal Price { get; init; }
    public StockStatus StockStatus { get; init; }
}