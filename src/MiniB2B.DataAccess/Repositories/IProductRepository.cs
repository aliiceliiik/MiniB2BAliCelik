using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.DataAccess.Repositories;

public interface IProductRepository
{
    Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request);
    Task<ProductStockInfoDto?> GetStockInfoAsync(int productId);
}