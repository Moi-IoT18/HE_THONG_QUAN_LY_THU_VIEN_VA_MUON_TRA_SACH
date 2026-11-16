// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Khai báo mã quyền dùng chung, bao gồm quyền quản lý Bạn đọc cho Admin và nhân viên.

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers
{
    using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

    public static class Quyen
    {
        public const string BanDoc = "BanDoc";
        public const string NhanVien = "NhanVien";
        public const string ThuThu = "ThuThu";
        public const string QuanLy = "QuanLy";
        public const string NhanVienVaQuanLy = NhanVien + "," + QuanLy;

        public static string TuVaiTro(VaiTro vaiTro) => vaiTro switch
        {
            VaiTro.BanDoc => BanDoc,
            VaiTro.ThuThu => NhanVien,
            VaiTro.QuanLy => QuanLy,
            _ => BanDoc,
        };
    }
}
