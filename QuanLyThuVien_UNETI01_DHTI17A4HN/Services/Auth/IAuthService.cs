// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Khai báo các nghiệp vụ xác thực và đăng ký bạn đọc.

using TaiKhoanModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;

public interface IAuthService
{
    Task<TaiKhoanModel?> LoginAsync(string tenDangNhap, string matKhau);
    Task<bool> RegisterReaderAsync(DangKyViewModel model);
}
