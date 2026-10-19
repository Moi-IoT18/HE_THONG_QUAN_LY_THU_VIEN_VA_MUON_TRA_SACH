using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

public static partial class DbSeeder
{
    private const string MatKhauBanDoc = "BanDoc@123";

    private static User TaoTaiKhoan(
        string tenDangNhap,
        string matKhau,
        string hoTen,
        string email,
        VaiTro vaiTro,
        bool hoatDong = true) => new()
        {
            TenDangNhap = tenDangNhap,
            MatKhau = MatKhauHelper.BamMatKhau(matKhau),
            HoTen = hoTen,
            Email = email,
            VaiTro = vaiTro,
            TrangThai = hoatDong,
            NgayTao = DateTime.Now,
        };
}
