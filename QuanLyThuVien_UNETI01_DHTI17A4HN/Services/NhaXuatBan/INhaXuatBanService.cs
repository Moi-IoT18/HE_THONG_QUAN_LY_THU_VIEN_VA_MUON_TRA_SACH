// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Module 2 – Interface nghiệp vụ Nhà xuất bản.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public interface INhaXuatBanService
{
    Task<NhaXuatBanIndexViewModel> LayDanhSachAsync(string? tuKhoa, bool? trangThai, int trang);
    Task<NhaXuatBan?> LayChiTietAsync(int id);
    Task<NhaXuatBan?> LayTheoMaAsync(int id);
    Task<int> DemSachAsync(int id);
    Task<ServiceResult> TaoAsync(NhaXuatBan nxb);
    Task<ServiceResult> CapNhatAsync(int id, NhaXuatBan nxb);
    Task<ServiceResult> XoaAsync(int id);
}
