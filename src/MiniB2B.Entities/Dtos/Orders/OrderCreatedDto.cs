namespace MiniB2B.Entities.Dtos.Orders;

public class OrderCreatedDto
{
    public int OrderId { get; init; }
    public int OrderNumber { get; init; }
    public decimal TotalAmount { get; init; }
}