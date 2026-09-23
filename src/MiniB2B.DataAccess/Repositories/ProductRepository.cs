using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;
using MiniB2B.DataAccess.Extensions;

namespace MiniB2B.DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MiniB2BDbContext _context;

    public ProductRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request)
    {
        IQueryable<Product> query = _context.Products.Where(p => p.IsActive);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = ApplySearch(query, request.SearchTerm);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                ProductCode = p.ProductCode,
                Name = p.Name,
                Brand = p.Brand,
                Price = p.Price,
                StockStatus = p.StockQuantity <= 0 ? StockStatus.OutOfStock
                            : p.StockQuantity <= p.CriticalStockLevel ? StockStatus.Critical
                            : StockStatus.Available
            })
            .ToListAsync();

        return new PagedResult<ProductListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
    public Task<ProductStockInfoDto?> GetStockInfoAsync(int productId)
    {
        return _context.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductStockInfoDto
            {
                Id = p.Id,
                Name = p.Name,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync();
    }
    public async Task<bool> TryDecreaseStockAsync(int productId, int quantity)
    {
        var affectedRows = await _context.Products
            .Where(p => p.Id == productId && p.IsActive && p.StockQuantity >= quantity)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.StockQuantity, p => p.StockQuantity - quantity)
                .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

        return affectedRows == 1;
    }

    public Task<PagedResult<AdminProductListItemDto>> SearchForAdminAsync(AdminProductSearchRequest request)
    {
        IQueryable<Product> query = _context.Products;

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = ApplySearch(query, request.SearchTerm);

        return query
            .OrderBy(p => p.ProductCode)
            .Select(p => new AdminProductListItemDto
            {
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                ProductCode = p.ProductCode,
                Name = p.Name,
                Brand = p.Brand,
                CategoryName = p.Category.Name,
                StockQuantity = p.StockQuantity,
                CriticalStockLevel = p.CriticalStockLevel,
                Price = p.Price,
                IsActive = p.IsActive
            })
            .ToPagedResultAsync(request.Page, request.PageSize);
    }
    public Task<ProductFormDto?> GetFormAsync(int id)
    {
        return _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductFormDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                ProductCode = p.ProductCode,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                ManufacturerCode = p.ManufacturerCode,
                SpecialCode1 = p.SpecialCode1,
                SpecialCode2 = p.SpecialCode2,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                CriticalStockLevel = p.CriticalStockLevel,
                Price = p.Price,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public Task<bool> ProductCodeExistsAsync(string productCode, int? excludeId)
    {
        var query = _context.Products.Where(p => p.ProductCode == productCode);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return query.AnyAsync();
    }

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<ProductDetailDto?> GetDetailAsync(int id)
    {
        return _context.Products
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                ManufacturerCode = p.ManufacturerCode,
                SpecialCode1 = p.SpecialCode1,
                SpecialCode2 = p.SpecialCode2,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                Price = p.Price,
                StockStatus = p.StockQuantity <= 0 ? StockStatus.OutOfStock
                            : p.StockQuantity <= p.CriticalStockLevel ? StockStatus.Critical
                            : StockStatus.Available
            })
            .FirstOrDefaultAsync();
    }

    private static IQueryable<Product> ApplySearch(IQueryable<Product> query, string term)
    {
        return query.Where(p =>
            p.ProductCode.Contains(term) ||
            p.Name.Contains(term) ||
            p.Brand.Contains(term) ||
            (p.ManufacturerCode != null && p.ManufacturerCode.Contains(term)) ||
            (p.Description != null && p.Description.Contains(term)) ||
            (p.SpecialCode1 != null && p.SpecialCode1.Contains(term)) ||
            (p.SpecialCode2 != null && p.SpecialCode2.Contains(term)));
    }
}