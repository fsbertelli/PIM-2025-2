using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace API.Services;

public interface IFileStorageService
{
    Task<string?> SaveBase64Async(string base64, IWebHostEnvironment env);
    Task<string?> SaveFormFileAsync(IFormFile file, IWebHostEnvironment env);
}

public partial class FileStorageService : IFileStorageService
{
    public async Task<string?> SaveBase64Async(string base64, IWebHostEnvironment env)
    {
        if (string.IsNullOrWhiteSpace(base64)) return null;

        try
        {
            var clean = base64.Trim();
            string? extension = null;

            var commaIndex = clean.IndexOf(',', StringComparison.Ordinal);
            if (clean.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex > 0)
            {
                var meta = clean.Substring(5, commaIndex - 5);
                var semiIndex = meta.IndexOf(';');
                var mime = semiIndex > 0 ? meta.Substring(0, semiIndex) : meta;
                extension = mime switch
                {
                    "image/png" => ".png",
                    "image/jpeg" => ".jpg",
                    "image/jpg" => ".jpg",
                    "image/gif" => ".gif",
                    "image/webp" => ".webp",
                    _ => null
                };
                clean = clean.Substring(commaIndex + 1);
            }

            byte[] bytes = Convert.FromBase64String(clean);

            if (extension is null)
            {
                if (bytes.Length >= 4 && bytes[0] == 0x89 && bytes[1] == 0x50) extension = ".png";
                else if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8) extension = ".jpg";
                else extension = ".bin";
            }

            var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsRoot = Path.Combine(webRoot, "uploads");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);
            await File.WriteAllBytesAsync(filePath, bytes);

            return $"/uploads/{fileName}";
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> SaveFormFileAsync(IFormFile file, IWebHostEnvironment env)
    {
        if (file is null || file.Length == 0) return null;

        try
        {
            var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsRoot = Path.Combine(webRoot, "uploads");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            // Determine extension from content type or filename
            var ext = file.ContentType?.ToLower() switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => Path.GetExtension(file.FileName) ?? ".img"
            };

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // Optionally resize using ImageSharp? Keep original for now (other endpoints may resize)
            await File.WriteAllBytesAsync(filePath, bytes);

            return $"/uploads/{fileName}";
        }
        catch
        {
            return null;
        }
    }
}
