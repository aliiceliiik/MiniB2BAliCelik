using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;

namespace MiniB2B.Business.Services;

public interface IOrderService
{
    Task<ServiceResult<OrderCreatedDto>> CreateOrderAsync(int userId);
}