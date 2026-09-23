using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Business.Services;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request);
    Task<ProductDetailDto?> GetDetailAsync(int id);
}