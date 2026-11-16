// Họ và tên: Nguyễn Thế Đông
// Mã sinh viên: 23103100187
// Nội dung thực hiện: Xác thực tài khoản hoạt động bằng mật khẩu đã băm.

using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
using TaiKhoanModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ITaiKhoanRepository _taiKhoanRepository;

    public AuthService(ITaiKhoanRepository taiKhoanRepository)
    {
        _taiKhoanRepository = taiKhoanRepository;
    }

    public async Task<TaiKhoanModel?> LoginAsync(string tenDangNhap, string matKhau)
    {
        var taiKhoan = await _taiKhoanRepository.GetByTenDangNhapAsync(tenDangNhap);
        if (taiKhoan is not { TrangThai: true }) return null;

        var passwordMatches = MatKhauHelper.KiemTra(matKhau, taiKhoan.MatKhau);
        return passwordMatches ? taiKhoan : null;
    }

    public async Task<bool> RegisterReaderAsync(DangKyViewModel model)
    {
        var tenDangNhap = model.TenDangNhap.Trim();
        if (await _taiKhoanRepository.ExistsAsync(tenDangNhap))
            return false;

        var hoTen = model.HoTen.Trim();
        var email = model.Email.Trim();
        var ngayCapThe = DateTime.Today;
        var taiKhoan = new TaiKhoanModel
        {
            TenDangNhap = tenDangNhap,
            MatKhau = MatKhauHelper.BamMatKhau(model.MatKhau),
            HoTen = hoTen,
            Email = email,
            VaiTro = VaiTro.BanDoc,
            TrangThai = true,
            NgayTao = DateTime.Now,
            BanDoc = new BanDoc
            {
                HoTen = hoTen,
                Email = email,
                NgayCapThe = ngayCapThe,
                HanThe = ngayCapThe.AddYears(1),
                TrangThai = TrangThaiBanDoc.HoatDong,
            },
        };

        try
        {
            await _taiKhoanRepository.AddAsync(taiKhoan);
            await _taiKhoanRepository.SaveChangesAsync();
            return true;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return false;
        }
    }
}
