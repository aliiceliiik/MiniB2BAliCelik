namespace MiniB2B.Entities.Dtos.Products;

public class AdminProductListItemDto
{
    public int Id { get; init; }
    public string? ImageUrl { get; init; }
    public string ProductCode { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string CategoryName { get; init; } = null!;
    public int StockQuantity { get; init; }
    public int CriticalStockLevel { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
}