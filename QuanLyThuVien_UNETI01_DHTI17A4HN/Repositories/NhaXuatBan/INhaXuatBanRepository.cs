// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Module 2 – Interface truy cập dữ liệu Nhà xuất bản.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public interface INhaXuatBanRepository : IRepository<NhaXuatBan>
{
    IQueryable<NhaXuatBanDong> TruyVanDanhSach(string? tuKhoa, bool? trangThai);
    Task<NhaXuatBan?> LayChiTietAsync(int maNhaXuatBan);
    Task<bool> TrungTenAsync(string ten, int maBoQua = 0);
    Task<int> DemSachAsync(int maNhaXuatBan);
}
