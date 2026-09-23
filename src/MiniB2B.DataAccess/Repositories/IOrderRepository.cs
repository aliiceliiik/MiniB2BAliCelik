using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<PagedResult<OrderListItemDto>> GetOrdersAsync(int? userId, int page, int pageSize);
    Task<OrderDetailDto?> GetOrderDetailAsync(int orderId, int? userId);
    Task<PagedResult<OrderListItemDto>> SearchForAdminAsync(AdminOrderSearchRequest request);
    Task<bool> TryChangeStatusAsync(int orderId, OrderStatus expectedStatus, OrderStatus newStatus);
    Task<int> RestockItemsAsync(int orderId);
}