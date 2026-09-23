using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Categories;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business.Services;

public class AdminProductService : IAdminProductService
{
    private const int MaxPageSize = 100;

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AdminProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public Task<PagedResult<AdminProductListItemDto>> SearchAsync(AdminProductSearchRequest request)
    {
        request.Page = Math.Max(1, request.Page);
        request.PageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        request.SearchTerm = NullIfEmpty(request.SearchTerm);

        return _productRepository.SearchForAdminAsync(request);
    }

    public Task<ProductFormDto?> GetForEditAsync(int id) => _productRepository.GetFormAsync(id);

    public Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync() => _categoryRepository.GetOptionsAsync();

    public async Task<ServiceResult<int>> CreateAsync(ProductFormDto form)
    {
        Normalize(form);

        var error = await ValidateAsync(form, excludeId: null);
        if (error is not null)
            return ServiceResult<int>.Failure(error);

        var product = new Product { ImageUrl = form.ImageUrl };
        MapToEntity(form, product);

        await _productRepository.AddAsync(product);
        return ServiceResult<int>.Success(product.Id);
    }

    public async Task<ServiceResult> UpdateAsync(ProductFormDto form)
    {
        var product = await _productRepository.GetByIdAsync(form.Id);
        if (product is null)
            return ServiceResult.Failure("Ürün bulunamadı.");

        Normalize(form);

        var error = await ValidateAsync(form, excludeId: form.Id);
        if (error is not null)
            return ServiceResult.Failure(error);

        MapToEntity(form, product);

        if (form.ImageUrl is not null)
            product.ImageUrl = form.ImageUrl;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void Normalize(ProductFormDto form)
    {
        form.ProductCode = form.ProductCode.Trim();
        form.Name = form.Name.Trim();
        form.Brand = form.Brand.Trim();
        form.Description = NullIfEmpty(form.Description);
        form.ManufacturerCode = NullIfEmpty(form.ManufacturerCode);
        form.SpecialCode1 = NullIfEmpty(form.SpecialCode1);
        form.SpecialCode2 = NullIfEmpty(form.SpecialCode2);
    }

    private async Task<string?> ValidateAsync(ProductFormDto form, int? excludeId)
    {
        if (form.Price <= 0)
            return "Fiyat 0'dan büyük olmalıdır.";

        if (form.StockQuantity < 0 || form.CriticalStockLevel < 0)
            return "Stok değerleri negatif olamaz.";

        if (!await _categoryRepository.ExistsAsync(form.CategoryId))
            return "Seçilen kategori bulunamadı.";

        if (await _productRepository.ProductCodeExistsAsync(form.ProductCode, excludeId))
            return $"'{form.ProductCode}' ürün kodu başka bir üründe kullanılıyor.";

        return null;
    }

    private static void MapToEntity(ProductFormDto form, Product product)
    {
        product.CategoryId = form.CategoryId;
        product.ProductCode = form.ProductCode;
        product.Name = form.Name;
        product.Description = form.Description;
        product.Brand = form.Brand;
        product.ManufacturerCode = form.ManufacturerCode;
        product.SpecialCode1 = form.SpecialCode1;
        product.SpecialCode2 = form.SpecialCode2;
        product.StockQuantity = form.StockQuantity;
        product.CriticalStockLevel = form.CriticalStockLevel;
        product.Price = form.Price;
        product.IsActive = form.IsActive;
    }
}