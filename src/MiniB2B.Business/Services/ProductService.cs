using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Business.Services;

public class ProductService : IProductService
{
    private const int MaxPageSize = 100;
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request)
    {
        request.Page = Math.Max(1, request.Page);
        request.PageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        request.SearchTerm = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? null
            : request.SearchTerm.Trim();

        return _productRepository.SearchAsync(request);
    }

    public Task<ProductDetailDto?> GetDetailAsync(int id) => _productRepository.GetDetailAsync(id);
}