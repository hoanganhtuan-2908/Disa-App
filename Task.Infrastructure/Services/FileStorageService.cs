using Microsoft.AspNetCore.Http;
using Task.Application.Interfaces.Services;

namespace Task.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    public async System.Threading.Tasks.Task<string> UploadImageAsync(IFormFile file)
    {
        var folder = Path.Combine("Uploads", "Images");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);

        await file.CopyToAsync(stream);

        return path;
    }

    public async System.Threading.Tasks.Task<string> UploadVideoAsync(IFormFile file)
    {
        var folder = Path.Combine("Uploads", "Videos");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);

        await file.CopyToAsync(stream);

        return path;
    }
}