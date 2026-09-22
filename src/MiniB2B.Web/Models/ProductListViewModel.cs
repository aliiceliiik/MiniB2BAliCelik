using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Grid;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Web.Models;

public class ProductListViewModel
{
    public IReadOnlyList<GridColumnDto> Columns { get; init; } = [];
    public PagedResult<ProductListItemDto> Products { get; init; } = new();
    public string? SearchTerm { get; init; }
}