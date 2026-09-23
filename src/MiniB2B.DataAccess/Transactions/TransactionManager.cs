using Microsoft.EntityFrameworkCore.Storage;
using MiniB2B.DataAccess.Context;

namespace MiniB2B.DataAccess.Transactions;

public class TransactionManager : ITransactionManager
{
    private readonly MiniB2BDbContext _context;

    public TransactionManager(MiniB2BDbContext context)
    {
        _context = context;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }
}