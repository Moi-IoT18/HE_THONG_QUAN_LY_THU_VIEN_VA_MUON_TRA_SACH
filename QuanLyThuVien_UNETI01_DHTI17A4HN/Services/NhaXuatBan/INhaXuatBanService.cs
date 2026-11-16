// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Hợp đồng nghiệp vụ CRUD nhà xuất bản và kiểm soát tham chiếu (Module 2).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public interface INhaXuatBanService
{
    Task<List<NhaXuatBan>> GetAllAsync();
    Task<NhaXuatBan?> GetByIdAsync(int id);
    Task<bool> IsDuplicateAsync(string tenNhaXuatBan, int? excludeId = null);
    Task<bool> IsInUseAsync(int id);
    Task AddAsync(NhaXuatBan entity);
    Task UpdateAsync(NhaXuatBan entity);
    Task<bool> DeleteAsync(int id);
}
