using MiniB2B.Entities.Dtos.Common;

namespace MiniB2B.Web.Services;

public class LocalImageStorage : IImageStorage
{
    private const long MaxFileSize = 2 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly IWebHostEnvironment _environment;

    public LocalImageStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<ServiceResult<string>> SaveAsync(IFormFile file, string folderName)
    {
        if (file.Length == 0)
            return ServiceResult<string>.Failure("Boş bir dosya yüklenemez.");

        if (file.Length > MaxFileSize)
            return ServiceResult<string>.Failure("Resim boyutu en fazla 2 MB olabilir.");

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension) || !await HasImageSignatureAsync(file))
            return ServiceResult<string>.Failure("Sadece JPG, PNG veya WEBP formatında resim yüklenebilir.");

        var folder = Path.Combine(_environment.WebRootPath, "uploads", folderName);
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(folder, fileName);

        await using var output = new FileStream(fullPath, FileMode.CreateNew);
        await file.CopyToAsync(output);

        return ServiceResult<string>.Success($"/uploads/{folderName}/{fileName}");
    }

    private static async Task<bool> HasImageSignatureAsync(IFormFile file)
    {
        var header = new byte[12];

        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);

        if (read < 4)
            return false;

        var isJpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        var isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
        var isWebp = read >= 12
                     && header[0] == 'R' && header[1] == 'I' && header[2] == 'F' && header[3] == 'F'
                     && header[8] == 'W' && header[9] == 'E' && header[10] == 'B' && header[11] == 'P';

        return isJpeg || isPng || isWebp;
    }
}