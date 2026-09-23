using Microsoft.EntityFrameworkCore.Storage;

namespace MiniB2B.DataAccess.Transactions;

public interface ITransactionManager
{
    Task<IDbContextTransaction> BeginTransactionAsync();
}