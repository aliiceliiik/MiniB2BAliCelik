using MiniB2B.Business.Common;
using MiniB2B.DataAccess.Repositories;
using MiniB2B.DataAccess.Transactions;
using MiniB2B.Entities.Dtos.Cart;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business.Services;

public class OrderService : IOrderService
{
    private readonly ITransactionManager _transactionManager;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        ITransactionManager transactionManager,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _transactionManager = transactionManager;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    private static List<string> FindStockErrors(IEnumerable<CartItemDto> items)
    {
        return items
            .Where(i => !i.HasSufficientStock)
            .Select(i => i.IsActive
                ? StockMessages.Insufficient(i.ProductName, i.AvailableStock)
                : $"{i.ProductName} artık satışta değil.")
            .ToList();
    }

    public async Task<ServiceResult<OrderCreatedDto>> CreateOrderAsync(int userId)
    {
        await using var transaction = await _transactionManager.BeginTransactionAsync();

        var items = await _cartRepository.GetItemsAsync(userId);

        if (items.Count == 0)
            return ServiceResult<OrderCreatedDto>.Failure("Sepetiniz boş. Sipariş oluşturmak için sepetinize ürün ekleyin.");

        var stockErrors = FindStockErrors(items);
        if (stockErrors.Count > 0)
            return ServiceResult<OrderCreatedDto>.Failure(string.Join(" ", stockErrors));

        foreach (var item in items.OrderBy(i => i.ProductId))
        {
            var decreased = await _productRepository.TryDecreaseStockAsync(item.ProductId, item.Quantity);

            if (!decreased)
                return ServiceResult<OrderCreatedDto>.Failure(
                    $"{item.ProductName} için stok siz işlem yaparken değişti. Lütfen sepetinizi kontrol edip tekrar deneyin.");
        }

        var order = new Order
        {
            UserId = userId,
            TotalAmount = items.Sum(i => i.LineTotal),
            Items = items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductCode = i.ProductCode,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _orderRepository.AddAsync(order);

        var clearedCount = await _cartRepository.ClearAsync(userId);
        if (clearedCount != items.Count)
            return ServiceResult<OrderCreatedDto>.Failure(
                "Sepetiniz sipariş sırasında değişti. Lütfen sepetinizi kontrol edip tekrar deneyin.");

        await transaction.CommitAsync();

        return ServiceResult<OrderCreatedDto>.Success(new OrderCreatedDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount
        });
    }
}