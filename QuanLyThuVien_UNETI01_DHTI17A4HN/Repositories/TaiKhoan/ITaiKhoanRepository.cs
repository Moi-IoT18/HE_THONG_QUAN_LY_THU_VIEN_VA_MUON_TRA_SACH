using TaiKhoanModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;

public interface ITaiKhoanRepository
{
    Task<TaiKhoanModel?> GetByTenDangNhapAsync(string tenDangNhap);
    Task<List<TaiKhoanModel>> GetAllAsync();
    Task<TaiKhoanModel?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(string tenDangNhap, int? excludeId = null);
    Task AddAsync(TaiKhoanModel taiKhoan);
    Task UpdateAsync(TaiKhoanModel taiKhoan);
    Task SaveChangesAsync();
}
