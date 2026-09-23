using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Models;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Orders;

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
    public async Task<PagedResult<OrderListItemDto>> GetOrdersAsync(int? userId, int page, int pageSize)
    {
        var query = _context.Orders.AsQueryable();

        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListItemDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CustomerName = o.User.FirstName + " " + o.User.LastName,
                ItemCount = o.Items.Count
            })
            .ToListAsync();

        return new PagedResult<OrderListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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
}