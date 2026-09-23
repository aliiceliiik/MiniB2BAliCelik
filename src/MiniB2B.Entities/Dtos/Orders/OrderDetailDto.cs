using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Orders;

public class OrderDetailDto
{
    public int Id { get; init; }
    public int OrderNumber { get; init; }
    public DateTime OrderDate { get; init; }
    public OrderStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public string CustomerName { get; init; } = null!;
    public string CustomerEmail { get; init; } = null!;
    public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
}