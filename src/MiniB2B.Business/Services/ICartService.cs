using MiniB2B.Entities.Dtos.Cart;
using MiniB2B.Entities.Dtos.Common;

namespace MiniB2B.Business.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<ServiceResult<CartDto>> AddItemAsync(int userId, AddToCartRequest request);
    Task<ServiceResult<CartDto>> UpdateItemAsync(int userId, int productId, int quantity);
    Task<ServiceResult<CartDto>> RemoveItemAsync(int userId, int productId);
    Task<int> GetItemCountAsync(int userId);
}