namespace MiniB2B.Entities.Dtos.Cart;

public class CartDto
{
    public IReadOnlyList<CartItemDto> Items { get; init; } = [];

    public int ItemCount => Items.Count;
    public decimal TotalAmount => Items.Sum(i => i.LineTotal);
    public bool CanCheckout => Items.Count > 0 && Items.All(i => i.HasSufficientStock);
}