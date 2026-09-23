using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<PagedResult<OrderListItemDto>> GetOrdersAsync(int? userId, int page, int pageSize);
    Task<OrderDetailDto?> GetOrderDetailAsync(int orderId, int? userId);
}