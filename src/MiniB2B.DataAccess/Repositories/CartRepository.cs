using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Dtos.Cart;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public class CartRepository : ICartRepository
{
    private readonly MiniB2BDbContext _context;

    public CartRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public Task<int?> GetCartIdAsync(int userId)
    {
        return _context.Carts
            .Where(c => c.UserId == userId)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateCartAsync(int userId)
    {
        var cart = new Cart { UserId = userId };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        return cart.Id;
    }

    public Task<CartItem?> GetItemAsync(int userId, int productId)
    {
        return _context.CartItems
            .FirstOrDefaultAsync(i => i.Cart.UserId == userId && i.ProductId == productId);
    }

    public async Task<IReadOnlyList<CartItemDto>> GetItemsAsync(int userId)
    {
        return await _context.CartItems
            .Where(i => i.Cart.UserId == userId)
            .OrderBy(i => i.AddedAt)
            .Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductCode = i.Product.ProductCode,
                ProductName = i.Product.Name,
                ImageUrl = i.Product.ImageUrl,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity,
                AvailableStock = i.Product.StockQuantity,
                IsActive = i.Product.IsActive
            })
            .ToListAsync();
    }

    public Task<int> CountItemsAsync(int userId)
    {
        return _context.CartItems.CountAsync(i => i.Cart.UserId == userId);
    }

    public Task<int> ClearAsync(int userId)
    {
        return _context.CartItems
            .Where(i => i.Cart.UserId == userId)
            .ExecuteDeleteAsync();
    }
    public void AddItem(CartItem item) => _context.CartItems.Add(item);

    public void RemoveItem(CartItem item) => _context.CartItems.Remove(item);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}