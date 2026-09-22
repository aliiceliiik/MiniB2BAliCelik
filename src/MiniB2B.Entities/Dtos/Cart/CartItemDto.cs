namespace MiniB2B.Entities.Dtos.Cart;

public class CartItemDto
{
    public int ProductId { get; init; }
    public string ProductCode { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public string? ImageUrl { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public int AvailableStock { get; init; }
    public bool IsActive { get; init; }

    public decimal LineTotal => UnitPrice * Quantity;
    public bool HasSufficientStock => IsActive && Quantity <= AvailableStock;
}