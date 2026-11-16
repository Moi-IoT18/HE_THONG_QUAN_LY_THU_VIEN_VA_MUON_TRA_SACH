// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Khai báo truy vấn và thao tác dữ liệu sách (Module 2).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using SachEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Sach;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;

public interface ISachRepository
{
    IQueryable<SachEntity> TruyVanDanhSach(string? tuKhoa, int? maTheLoai, int? namXuatBan,
        TrangThaiSach? trangThai, bool? conSach, decimal? giaTu, decimal? giaDen, string sapXep);
    Task<SachEntity?> GetByIdAsync(int id);
    Task<bool> KhoaNgoaiHopLeAsync(int maTheLoai, int maNhaXuatBan, bool choPhepNgungHoatDong = false);
    Task<bool> IsInUseAsync(int id);
    Task AddAsync(SachEntity sach);
    Task UpdateAsync(SachEntity sach);
    Task DeleteAsync(SachEntity sach);
    Task SaveChangesAsync();
}
