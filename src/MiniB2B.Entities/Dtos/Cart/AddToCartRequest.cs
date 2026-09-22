using System.ComponentModel.DataAnnotations;

namespace MiniB2B.Entities.Dtos.Cart;

public class AddToCartRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Geçersiz ürün.")]
    public int ProductId { get; set; }

    [Range(1, 10000, ErrorMessage = "Adet 1 ile 10.000 arasında olmalıdır.")]
    public int Quantity { get; set; }
}