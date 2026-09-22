namespace MiniB2B.Entities.Dtos.Products;

public class ProductStockInfoDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
}