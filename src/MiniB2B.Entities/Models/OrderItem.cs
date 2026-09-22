namespace MiniB2B.Entities.Models;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; private set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}