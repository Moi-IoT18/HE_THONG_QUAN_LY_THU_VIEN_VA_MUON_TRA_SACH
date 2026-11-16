using TaiKhoanModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TaiKhoan;

public interface ITaiKhoanService
{
    Task<List<TaiKhoanModel>> GetAllAsync();
    Task<TaiKhoanModel?> GetByIdAsync(int id);
    Task<bool> IsDuplicateAsync(string tenDangNhap, int? excludeId = null);
    Task AddAsync(TaiKhoanModel taiKhoan);
    Task UpdateAsync(TaiKhoanModel taiKhoan);
}
