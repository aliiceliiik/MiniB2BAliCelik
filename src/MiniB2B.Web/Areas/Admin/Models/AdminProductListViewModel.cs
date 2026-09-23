using MiniB2B.Entities.Dtos.Categories;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Web.Areas.Admin.Models;

public class AdminProductListViewModel
{
    public PagedResult<AdminProductListItemDto> Products { get; init; } = new();
    public IReadOnlyList<CategoryOptionDto> Categories { get; init; } = [];
    public AdminProductSearchRequest Filter { get; init; } = new();
}