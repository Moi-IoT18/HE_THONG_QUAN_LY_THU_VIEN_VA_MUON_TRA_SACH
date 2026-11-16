// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Đọc đúng các giá trị ID integer và thông tin người dùng từ Session.

using Microsoft.AspNetCore.Http;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class SessionExtensions
{
    public static int? LayMaTaiKhoan(this ISession session)
        => session.GetInt32(SessionKeys.MaTaiKhoan);

    public static int? LayMaBanDoc(this ISession session)
        => session.GetInt32(SessionKeys.MaBanDoc);

    public static string? LayHoTen(this ISession session)
        => session.GetString(SessionKeys.HoTen);

    public static string? LayVaiTro(this ISession session)
        => session.GetString(SessionKeys.VaiTro);

    public static bool DaDangNhap(this ISession session)
        => session.LayMaTaiKhoan().HasValue;
}
