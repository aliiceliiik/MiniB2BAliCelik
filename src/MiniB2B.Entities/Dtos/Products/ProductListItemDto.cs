using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Products;

public class ProductListItemDto
{
    public int Id { get; init; }
    public string? ImageUrl { get; init; }
    public string ProductCode { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public StockStatus StockStatus { get; init; }
    public decimal Price { get; init; }
}