using MiniB2B.Entities.Dtos.Common;

namespace MiniB2B.Web.Services;

public interface IImageStorage
{
    Task<ServiceResult<string>> SaveProductImageAsync(IFormFile file);
}