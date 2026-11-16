using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;

namespace QuanLyThuVien.Tests;

public sealed class BookImageStorageTests
{
    [Fact]
    public async Task SaveAsync_StoresPngWithGeneratedNameAndCanDeleteIt()
    {
        var root = Path.Combine(Path.GetTempPath(), $"library-image-test-{Guid.NewGuid():N}");
        var storage = new BookImageStorage(new TestEnvironment(root));
        var bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 1, 2, 3 };
        await using var stream = new MemoryStream(bytes);
        var file = new FormFile(stream, 0, bytes.Length, "AnhBia", "unsafe-name.png");

        try
        {
            var savedPath = await storage.SaveAsync(file);

            Assert.StartsWith("/uploads/books/", savedPath, StringComparison.Ordinal);
            Assert.EndsWith(".png", savedPath, StringComparison.Ordinal);
            var savedFile = Path.Combine(root, "wwwroot",
                savedPath!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            Assert.Equal(bytes, await File.ReadAllBytesAsync(savedFile));
            await storage.DeleteAsync(savedPath);
            Assert.False(File.Exists(savedFile));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task SaveAsync_RejectsUnsupportedFileContent()
    {
        var root = Path.Combine(Path.GetTempPath(), $"library-image-test-{Guid.NewGuid():N}");
        var storage = new BookImageStorage(new TestEnvironment(root));
        await using var stream = new MemoryStream("<script>alert(1)</script>"u8.ToArray());
        var file = new FormFile(stream, 0, stream.Length, "AnhBia", "cover.png");

        try
        {
            await Assert.ThrowsAsync<InvalidDataException>(() => storage.SaveAsync(file));
            Assert.False(Directory.Exists(root));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    private sealed class TestEnvironment(string contentRoot) : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "QuanLyThuVien.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.Combine(contentRoot, "wwwroot");
        public string EnvironmentName { get; set; } = "Testing";
        public string ContentRootPath { get; set; } = contentRoot;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
