using MiniB2B.Entities.Dtos.Categories;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Business.Services;

public interface IAdminProductService
{
    Task<PagedResult<AdminProductListItemDto>> SearchAsync(AdminProductSearchRequest request);
    Task<ProductFormDto?> GetForEditAsync(int id);
    Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync();
    Task<ServiceResult<int>> CreateAsync(ProductFormDto form);
    Task<ServiceResult> UpdateAsync(ProductFormDto form);
}