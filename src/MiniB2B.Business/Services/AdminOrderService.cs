using MiniB2B.DataAccess.Repositories;
using MiniB2B.DataAccess.Transactions;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Business.Services;

public class AdminOrderService : IAdminOrderService
{
    private const int MaxPageSize = 100;

    private readonly IOrderRepository _orderRepository;
    private readonly ITransactionManager _transactionManager;

    public AdminOrderService(IOrderRepository orderRepository, ITransactionManager transactionManager)
    {
        _orderRepository = orderRepository;
        _transactionManager = transactionManager;
    }

    public Task<PagedResult<OrderListItemDto>> SearchAsync(AdminOrderSearchRequest request)
    {
        request.Page = Math.Max(1, request.Page);
        request.PageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        request.SearchTerm = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : request.SearchTerm.Trim();

        return _orderRepository.SearchForAdminAsync(request);
    }

    public Task<OrderDetailDto?> GetDetailAsync(int orderId)
    {
        return _orderRepository.GetOrderDetailAsync(orderId, userId: null);
    }

    public async Task<ServiceResult> ChangeStatusAsync(int orderId, OrderStatus newStatus)
    {
        if (newStatus is not (OrderStatus.Approved or OrderStatus.Rejected))
            return ServiceResult.Failure("Sipariş yalnızca Onaylandı veya Reddedildi durumuna getirilebilir.");

        await using var transaction = await _transactionManager.BeginTransactionAsync();

        var changed = await _orderRepository.TryChangeStatusAsync(orderId, OrderStatus.Pending, newStatus);
        if (!changed)
            return ServiceResult.Failure("Sipariş bulunamadı ya da daha önce sonuçlandırılmış.");

        if (newStatus == OrderStatus.Rejected)
            await _orderRepository.RestockItemsAsync(orderId);

        await transaction.CommitAsync();
        return ServiceResult.Success();
    }
}