// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Interface nghiệp vụ Bạn đọc (quản lý + hồ sơ cá nhân).
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public interface IBanDocService
{
    // ----- Quản lý (Thủ thư, Admin) -----
    Task<BanDocIndexViewModel> LayDanhSachAsync(string? tuKhoa, TrangThaiBanDoc? trangThai, string? tinhTrangThe, string sapXep, int trang);
    Task<BanDocChiTietViewModel?> LayChiTietAsync(int id);
    Task<BanDoc?> LayKemTaiKhoanAsync(int id);
    Task<int> DemPhieuAsync(int id);
    Task<BanDocFormViewModel?> LayFormSuaAsync(int id);
 
    /// <summary>Tạo bạn đọc kèm tài khoản đăng nhập. DuLieu = mã bạn đọc mới.</summary>
    Task<ServiceResult<int>> TaoAsync(BanDocFormViewModel vm);
    Task<ServiceResult> CapNhatAsync(int id, BanDocFormViewModel vm);
    Task<ServiceResult> KhoaMoAsync(int id);
    Task<ServiceResult> GiaHanTheAsync(int id);
    Task<ServiceResult> XoaAsync(int id);
 
    // ----- Hồ sơ cá nhân (Bạn đọc) – luôn theo mã tài khoản trong Session -----
    Task<HoSoViewModel?> LayHoSoAsync(int? maTaiKhoan);
 
    /// <summary>DuLieu = họ tên mới để Controller cập nhật Session.</summary>
    Task<ServiceResult<string>> CapNhatHoSoAsync(int? maTaiKhoan, HoSoViewModel vm);
    Task<ServiceResult> DoiMatKhauAsync(int? maTaiKhoan, DoiMatKhauViewModel vm);
}
