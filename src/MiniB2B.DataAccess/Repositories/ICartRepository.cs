using MiniB2B.Entities.Dtos.Cart;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface ICartRepository
{
    Task<int?> GetCartIdAsync(int userId);
    Task<int> CreateCartAsync(int userId);
    Task<CartItem?> GetItemAsync(int userId, int productId);
    void AddItem(CartItem item);
    void RemoveItem(CartItem item);
    Task SaveChangesAsync();
    Task<IReadOnlyList<CartItemDto>> GetItemsAsync(int userId);
    Task<int> CountItemsAsync(int userId);
    Task<int> ClearAsync(int userId);
}