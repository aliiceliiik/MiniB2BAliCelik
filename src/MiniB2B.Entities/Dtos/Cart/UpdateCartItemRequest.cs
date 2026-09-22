using System.ComponentModel.DataAnnotations;

namespace MiniB2B.Entities.Dtos.Cart;

public class UpdateCartItemRequest
{
    [Range(1, 10000, ErrorMessage = "Adet 1 ile 10.000 arasında olmalıdır.")]
    public int Quantity { get; set; }
}