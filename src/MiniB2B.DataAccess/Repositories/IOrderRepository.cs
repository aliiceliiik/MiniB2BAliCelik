using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
}