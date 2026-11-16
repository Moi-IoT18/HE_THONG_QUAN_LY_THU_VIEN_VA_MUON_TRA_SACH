namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;

public interface IBookImageStorage
{
    Task<string?> SaveAsync(IFormFile? image, CancellationToken cancellationToken = default);
    Task DeleteAsync(string? imagePath);
}
