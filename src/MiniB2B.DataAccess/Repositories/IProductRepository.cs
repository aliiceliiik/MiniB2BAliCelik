using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface IProductRepository
{
    Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request);
    Task<ProductStockInfoDto?> GetStockInfoAsync(int productId);
    Task<bool> TryDecreaseStockAsync(int productId, int quantity);
    Task<PagedResult<AdminProductListItemDto>> SearchForAdminAsync(AdminProductSearchRequest request);
    Task<ProductFormDto?> GetFormAsync(int id);
    Task<Product?> GetByIdAsync(int id);
    Task<bool> ProductCodeExistsAsync(string productCode, int? excludeId);
    Task AddAsync(Product product);
    Task SaveChangesAsync();
}