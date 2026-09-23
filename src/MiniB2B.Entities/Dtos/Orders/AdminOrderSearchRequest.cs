using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Orders;

public class AdminOrderSearchRequest
{
    public string? SearchTerm { get; set; }
    public OrderStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}