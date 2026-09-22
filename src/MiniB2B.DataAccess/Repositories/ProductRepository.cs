using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Entities.Enums;
using MiniB2B.Entities.Models;

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