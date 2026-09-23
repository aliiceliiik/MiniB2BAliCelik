using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Models;

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
}