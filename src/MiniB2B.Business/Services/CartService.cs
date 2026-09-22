using MiniB2B.Business.Common;
using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Cart;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var items = await _cartRepository.GetItemsAsync(userId);
        return new CartDto { Items = items };
    }

    public async Task<ServiceResult<CartDto>> AddItemAsync(int userId, AddToCartRequest request)
    {
        if (request.Quantity <= 0)
            return ServiceResult<CartDto>.Failure("Adet en az 1 olmalıdır.");

        var product = await _productRepository.GetStockInfoAsync(request.ProductId);
        var existingItem = await _cartRepository.GetItemAsync(userId, request.ProductId);
        var newQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        var error = ValidateStock(product, newQuantity);
        if (error is not null)
            return ServiceResult<CartDto>.Failure(error);

        if (existingItem is not null)
        {
            existingItem.Quantity = newQuantity;
        }
        else
        {
            var cartId = await _cartRepository.GetCartIdAsync(userId)
                         ?? await _cartRepository.CreateCartAsync(userId);

            _cartRepository.AddItem(new CartItem
            {
                CartId = cartId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }

        await _cartRepository.SaveChangesAsync();
        return ServiceResult<CartDto>.Success(await GetCartAsync(userId));
    }

    public async Task<ServiceResult<CartDto>> UpdateItemAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            return ServiceResult<CartDto>.Failure("Adet en az 1 olmalıdır.");

        var item = await _cartRepository.GetItemAsync(userId, productId);
        if (item is null)
            return ServiceResult<CartDto>.Failure(StockMessages.ItemNotInCart);

        var product = await _productRepository.GetStockInfoAsync(productId);
        var error = ValidateStock(product, quantity);
        if (error is not null)
            return ServiceResult<CartDto>.Failure(error);

        item.Quantity = quantity;
        await _cartRepository.SaveChangesAsync();

        return ServiceResult<CartDto>.Success(await GetCartAsync(userId));
    }

    public async Task<ServiceResult<CartDto>> RemoveItemAsync(int userId, int productId)
    {
        var item = await _cartRepository.GetItemAsync(userId, productId);
        if (item is null)
            return ServiceResult<CartDto>.Failure(StockMessages.ItemNotInCart);

        _cartRepository.RemoveItem(item);
        await _cartRepository.SaveChangesAsync();

        return ServiceResult<CartDto>.Success(await GetCartAsync(userId));
    }

    private static string? ValidateStock(ProductStockInfoDto? product, int requestedQuantity)
    {
        if (product is null || !product.IsActive)
            return StockMessages.ProductNotAvailable;

        if (requestedQuantity > product.StockQuantity)
            return StockMessages.Insufficient(product.Name, product.StockQuantity);

        return null;
    }
}