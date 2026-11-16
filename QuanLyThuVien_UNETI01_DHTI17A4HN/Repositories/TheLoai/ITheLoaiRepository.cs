using TheLoaiEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TheLoai;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TheLoai;

public interface ITheLoaiRepository
{
    Task<List<TheLoaiEntity>> GetAllAsync();
    Task<TheLoaiEntity?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(string tenTheLoai, int? excludeId = null);
    Task<bool> IsInUseAsync(int id);
    Task AddAsync(TheLoaiEntity entity);
    Task UpdateAsync(TheLoaiEntity entity);
    Task DeleteAsync(TheLoaiEntity entity);
    Task SaveChangesAsync();
}
