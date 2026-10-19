// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Dữ liệu mẫu Module 2 – 05 nhà xuất bản, 20 đầu sách nhiều trạng thái
//                     (còn/hết sách, ngừng phục vụ, nhiều năm xuất bản và mức giá).
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
 
public static partial class DbSeeder
{
    private static async Task SeedSachAsync(AppDbContext db, Random rd)
    {
        var nxbs = new List<NhaXuatBan>
        {
            new() { TenNhaXuatBan = "NXB Trẻ", DiaChi = "161B Lý Chính Thắng, Q.3, TP.HCM", SoDienThoai = "02839316289", Email = "hopthubandoc@nxbtre.com.vn" },
            new() { TenNhaXuatBan = "NXB Kim Đồng", DiaChi = "55 Quang Trung, Hai Bà Trưng, Hà Nội", SoDienThoai = "02439434730", Email = "info@nxbkimdong.com.vn" },
            new() { TenNhaXuatBan = "NXB Giáo dục Việt Nam", DiaChi = "81 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", SoDienThoai = "02438220801" },
            new() { TenNhaXuatBan = "NXB Tổng hợp TP.HCM", DiaChi = "62 Nguyễn Thị Minh Khai, Q.1, TP.HCM", SoDienThoai = "02838225340" },
            new() { TenNhaXuatBan = "NXB Khoa học và Kỹ thuật", DiaChi = "70 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", SoDienThoai = "02439423172" },
        };
        db.NhaXuatBans.AddRange(nxbs);
        await db.SaveChangesAsync();
 
        var tl = await db.TheLoais.ToDictionaryAsync(t => t.TenTheLoai, t => t.MaTheLoai);
        int Nxb(int i) => nxbs[i].MaNhaXuatBan;
 
        // (Tên sách, tác giả, thể loại, NXB, năm, đơn giá, kệ)
        var ds = new (string Ten, string TacGia, string TheLoai, int Nxb, int Nam, decimal Gia, string Ke)[]
        {
            ("Dế Mèn phiêu lưu ký", "Tô Hoài", "Thiếu nhi", 1, 2020, 55000, "TN-01"),
            ("Mắt biếc", "Nguyễn Nhật Ánh", "Văn học", 0, 2019, 110000, "VH-02"),
            ("Tôi thấy hoa vàng trên cỏ xanh", "Nguyễn Nhật Ánh", "Văn học", 0, 2018, 125000, "VH-02"),
            ("Cho tôi xin một vé đi tuổi thơ", "Nguyễn Nhật Ánh", "Văn học", 0, 2017, 80000, "VH-03"),
            ("Số đỏ", "Vũ Trọng Phụng", "Văn học", 3, 2016, 70000, "VH-01"),
            ("Tắt đèn", "Ngô Tất Tố", "Văn học", 3, 2015, 60000, "VH-01"),
            ("Đất rừng phương Nam", "Đoàn Giỏi", "Thiếu nhi", 1, 2021, 95000, "TN-02"),
            ("Hoàng tử bé", "Antoine de Saint-Exupéry", "Thiếu nhi", 1, 2022, 68000, "TN-02"),
            ("Lập trình C# căn bản", "Nhóm tác giả", "Công nghệ thông tin", 4, 2023, 150000, "CN-01"),
            ("Cấu trúc dữ liệu và giải thuật", "Đỗ Xuân Lôi", "Công nghệ thông tin", 4, 2021, 135000, "CN-01"),
            ("Cơ sở dữ liệu SQL Server", "Nhóm tác giả", "Công nghệ thông tin", 2, 2022, 120000, "CN-02"),
            ("Lập trình Web với ASP.NET Core", "Nhóm tác giả UNETI", "Công nghệ thông tin", 4, 2025, 180000, "CN-02"),
            ("Nhập môn Trí tuệ nhân tạo", "Từ Minh Phương", "Công nghệ thông tin", 4, 2024, 165000, "CN-03"),
            ("Kinh tế học vĩ mô", "N. Gregory Mankiw", "Kinh tế – Kỹ năng", 3, 2020, 210000, "KT-01"),
            ("Đắc nhân tâm", "Dale Carnegie", "Kinh tế – Kỹ năng", 3, 2019, 86000, "KT-02"),
            ("Nghĩ giàu và làm giàu", "Napoleon Hill", "Kinh tế – Kỹ năng", 3, 2018, 98000, "KT-02"),
            ("Việt Nam sử lược", "Trần Trọng Kim", "Lịch sử – Địa lý", 3, 2017, 145000, "LS-01"),
            ("Lược sử loài người", "Yuval Noah Harari", "Lịch sử – Địa lý", 3, 2021, 189000, "LS-01"),
            ("Giáo trình Lịch sử Đảng", "Bộ Giáo dục và Đào tạo", "Lịch sử – Địa lý", 2, 2015, 45000, "LS-02"),
            ("Tin học văn phòng 2010", "Nhóm tác giả", "Công nghệ thông tin", 2, 2012, 40000, "CN-04"),
        };
 
        foreach (var s in ds)
        {
            var soLuong = rd.Next(3, 9);
            db.Sachs.Add(new Sach
            {
                TenSach = s.Ten,
                TacGia = s.TacGia,
                MaTheLoai = tl[s.TheLoai],
                MaNhaXuatBan = Nxb(s.Nxb),
                NamXuatBan = s.Nam,
                SoLuong = soLuong,
                SoLuongCon = soLuong,
                DonGia = s.Gia,
                ViTriKeSach = s.Ke,
                TrangThai = s.Ten is "Giáo trình Lịch sử Đảng" or "Tin học văn phòng 2010"
                    ? TrangThaiSach.NgungPhucVu
                    : TrangThaiSach.ConPhucVu,
                GhiChu = s.Nam < 2014 ? "Bản cũ, chờ thanh lý" : null,
            });
        }
 
        await db.SaveChangesAsync();
    }
}
