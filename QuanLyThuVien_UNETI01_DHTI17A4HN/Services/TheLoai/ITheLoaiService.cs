using TheLoaiEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TheLoai;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TheLoai;

public interface ITheLoaiService
{
    Task<List<TheLoaiEntity>> GetAllAsync();
    Task<TheLoaiEntity?> GetByIdAsync(int id);
    Task<bool> IsDuplicateAsync(string tenTheLoai, int? excludeId = null);
    Task<bool> IsInUseAsync(int id);
    Task AddAsync(TheLoaiEntity entity);
    Task UpdateAsync(TheLoaiEntity entity);
    Task DeleteAsync(TheLoaiEntity entity);
}
