namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;

public sealed class BookImageStorage : IBookImageStorage
{
    private const long MaxImageBytes = 5 * 1024 * 1024;
    private const string RelativeDirectory = "uploads/books";
    private readonly string _webRootPath;

    public BookImageStorage(IWebHostEnvironment environment)
    {
        _webRootPath = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");
    }

    public async Task<string?> SaveAsync(IFormFile? image, CancellationToken cancellationToken = default)
    {
        if (image is null || image.Length == 0)
            return null;
        if (image.Length > MaxImageBytes)
            throw new InvalidDataException("Ảnh bìa không được vượt quá 5 MB.");

        var extension = await DetectImageAsync(image, cancellationToken);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var directory = Path.Combine(_webRootPath, RelativeDirectory.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, fileName);
        var created = false;
        var saved = false;
        try
        {
            await using (var destination = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;
                await image.CopyToAsync(destination, cancellationToken);
            }
            saved = true;
        }
        finally
        {
            if (created && !saved)
                File.Delete(filePath);
        }

        return $"/{RelativeDirectory}/{fileName}";
    }

    public Task DeleteAsync(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return Task.CompletedTask;

        const string prefix = $"/{RelativeDirectory}/";
        var fileName = imagePath.StartsWith(prefix, StringComparison.Ordinal)
            ? imagePath[prefix.Length..]
            : string.Empty;
        if (fileName.Length == 0 || fileName != Path.GetFileName(fileName))
            throw new InvalidOperationException("Book image path is outside the managed upload directory.");

        var filePath = Path.Combine(_webRootPath, RelativeDirectory.Replace('/', Path.DirectorySeparatorChar), fileName);
        File.Delete(filePath);
        return Task.CompletedTask;
    }

    private static async Task<string> DetectImageAsync(
        IFormFile image, CancellationToken cancellationToken)
    {
        var header = new byte[12];
        var read = 0;
        await using (var stream = image.OpenReadStream())
        {
            while (read < header.Length)
            {
                var count = await stream.ReadAsync(header.AsMemory(read), cancellationToken);
                if (count == 0)
                    break;
                read += count;
            }
        }

        if (read >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
            return ".png";
        if (read >= 3 && header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff)
            return ".jpg";
        if (read >= 12
            && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
            && header.AsSpan(8, 4).SequenceEqual("WEBP"u8))
            return ".webp";

        throw new InvalidDataException("Tệp tải lên không phải ảnh PNG, JPEG hoặc WebP hợp lệ.");
    }
}
