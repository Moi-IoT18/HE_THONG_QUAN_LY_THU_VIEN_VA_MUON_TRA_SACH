// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Dữ liệu mẫu Module 3 – 30 bạn đọc kèm tài khoản (bandoc01 … bandoc30),
//                     có thẻ hết hạn, thẻ tạm khóa và tài khoản bị khóa để kiểm thử.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
 
public static partial class DbSeeder
{
    private static readonly string[] Ho = ["Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Vũ", "Đặng", "Bùi", "Đỗ", "Ngô"];
    private static readonly string[] TenDem = ["Văn", "Thị", "Minh", "Thanh", "Ngọc", "Đức", "Thu", "Gia", "Hải", "Quốc"];
    private static readonly string[] Ten = ["An", "Bình", "Chi", "Dũng", "Giang", "Hà", "Hùng", "Linh", "Mai", "Nam", "Phương", "Quân", "Sơn", "Trang", "Tuấn"];
    private static readonly string[] Quan = ["Hai Bà Trưng", "Hoàng Mai", "Đống Đa", "Thanh Xuân", "Cầu Giấy", "Long Biên"];
 
    private static async Task SeedBanDocAsync(AppDbContext db, Random rd)
    {
        for (var i = 1; i <= 30; i++)
        {
            var nu = rd.Next(2) == 0;
            var hoTen = $"{Ho[rd.Next(Ho.Length)]} {(nu ? "Thị" : TenDem[rd.Next(TenDem.Length)])} {Ten[rd.Next(Ten.Length)]}";
            var tenDangNhap = $"bandoc{i:00}";
            var email = $"{tenDangNhap}@sv.uneti.edu.vn";
 
            // bandoc29: tài khoản bị khóa – dùng để kiểm thử đăng nhập
            var taiKhoan = TaoTaiKhoan(tenDangNhap, MatKhauBanDoc, hoTen, email, VaiTro.BanDoc, hoatDong: i != 29);
 
            var ngayCap = DateTime.Today.AddDays(-rd.Next(60, 700));
            var banDoc = new BanDoc
            {
                TaiKhoan = taiKhoan,
                HoTen = hoTen,
                NgaySinh = new DateTime(rd.Next(1998, 2007), rd.Next(1, 13), rd.Next(1, 28)),
                GioiTinh = nu ? GioiTinh.Nu : GioiTinh.Nam,
                SoDienThoai = "09" + rd.Next(10000000, 99999999),
                Email = email,
                DiaChi = $"{rd.Next(1, 300)} phố {Ten[rd.Next(Ten.Length)]}, {Quan[rd.Next(Quan.Length)]}, Hà Nội",
                NgayCapThe = ngayCap,
                HanThe = ngayCap.AddYears(2),
                TrangThai = TrangThaiBanDoc.HoatDong,
            };
 
            // bandoc26 – bandoc28: thẻ đã hết hạn
            if (i is >= 26 and <= 28)
            {
                banDoc.NgayCapThe = DateTime.Today.AddYears(-2).AddDays(-rd.Next(10, 90));
                banDoc.HanThe = DateTime.Today.AddDays(-rd.Next(5, 60));
                banDoc.GhiChu = "Thẻ hết hạn, cần gia hạn";
            }
 
            // bandoc30: thẻ tạm khóa
            if (i == 30)
            {
                banDoc.TrangThai = TrangThaiBanDoc.TamKhoa;
                banDoc.GhiChu = "Tạm khóa do làm mất sách nhiều lần";
            }
 
            db.BanDocs.Add(banDoc);
        }
 
        await db.SaveChangesAsync();
    }
}
