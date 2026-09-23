using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Business.Services;

public interface IAdminOrderService
{
    Task<PagedResult<OrderListItemDto>> SearchAsync(AdminOrderSearchRequest request);
    Task<OrderDetailDto?> GetDetailAsync(int orderId);
    Task<ServiceResult> ChangeStatusAsync(int orderId, OrderStatus newStatus);
}