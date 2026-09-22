namespace MiniB2B.Entities.Dtos.Products;

public class ProductSearchRequest
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}