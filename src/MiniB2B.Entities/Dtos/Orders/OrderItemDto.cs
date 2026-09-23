namespace MiniB2B.Entities.Dtos.Orders;

public class OrderItemDto
{
    public string ProductCode { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
}