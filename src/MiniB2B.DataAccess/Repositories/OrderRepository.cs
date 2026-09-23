using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Models;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;
using System.Linq.Expressions;
using MiniB2B.DataAccess.Extensions;
using MiniB2B.Entities.Enums;

namespace MiniB2B.DataAccess.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MiniB2BDbContext _context;

    public OrderRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
    public Task<PagedResult<OrderListItemDto>> GetOrdersAsync(int? userId, int page, int pageSize)
    {
        var query = _context.Orders.AsQueryable();

        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        return query
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id)
            .Select(ListItemProjection)
            .ToPagedResultAsync(page, pageSize);
    }

    public Task<OrderDetailDto?> GetOrderDetailAsync(int orderId, int? userId)
    {
        var query = _context.Orders.Where(o => o.Id == orderId);

        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        return query
            .Select(o => new OrderDetailDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CustomerName = o.User.FirstName + " " + o.User.LastName,
                CustomerEmail = o.User.Email,
                Items = o.Items
                    .OrderBy(i => i.Id)
                    .Select(i => new OrderItemDto
                    {
                        ProductCode = i.ProductCode,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public Task<PagedResult<OrderListItemDto>> SearchForAdminAsync(AdminOrderSearchRequest request)
    {
        var query = _context.Orders.AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm;

            if (int.TryParse(term.TrimStart('#'), out var orderNumber))
                query = query.Where(o => o.OrderNumber == orderNumber);
            else
                query = query.Where(o =>
                    (o.User.FirstName + " " + o.User.LastName).Contains(term) ||
                    o.User.Email.Contains(term));
        }

        return query
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id)
            .Select(ListItemProjection)
            .ToPagedResultAsync(request.Page, request.PageSize);
    }

    public async Task<bool> TryChangeStatusAsync(int orderId, OrderStatus expectedStatus, OrderStatus newStatus)
    {
        var affectedRows = await _context.Orders
            .Where(o => o.Id == orderId && o.Status == expectedStatus)
            .ExecuteUpdateAsync(setters => setters.SetProperty(o => o.Status, newStatus));

        return affectedRows == 1;
    }

    public Task<int> RestockItemsAsync(int orderId)
    {
        return _context.Database.ExecuteSqlAsync($"""
        UPDATE p
        SET p.StockQuantity = p.StockQuantity + oi.Quantity,
            p.UpdatedAt = SYSUTCDATETIME()
        FROM dbo.Products AS p
        INNER JOIN dbo.OrderItems AS oi ON oi.ProductId = p.Id
        WHERE oi.OrderId = {orderId};
        """);
    }

    private static readonly Expression<Func<Order, OrderListItemDto>> ListItemProjection = o => new OrderListItemDto
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        OrderDate = o.OrderDate,
        TotalAmount = o.TotalAmount,
        Status = o.Status,
        CustomerName = o.User.FirstName + " " + o.User.LastName,
        ItemCount = o.Items.Count
    };
}