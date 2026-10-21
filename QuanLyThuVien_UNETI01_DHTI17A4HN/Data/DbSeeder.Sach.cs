// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Seed dữ liệu mẫu cho Nhà Xuất Bản và Sách (Module 2 - SV2).

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

public static class DbSeederSach
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var nxbSeeds = new List<NhaXuatBan>
        {
            new NhaXuatBan { TenNhaXuatBan = "NXB Giáo Dục Việt Nam", DiaChi = "81 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", SoDienThoai = "02438220801", Email = "contact@nxbgd.vn", TrangThai = true },
            new NhaXuatBan { TenNhaXuatBan = "NXB Khoa Học và Kỹ Thuật", DiaChi = "70 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", SoDienThoai = "02439423362", Email = "nxbkhkt@gmail.com", TrangThai = true },
            new NhaXuatBan { TenNhaXuatBan = "NXB Trẻ", DiaChi = "161B Lý Chính Thắng, Võ Thị Sáu, Quận 3, TP.HCM", SoDienThoai = "02839316289", Email = "hopthubaonxb@nxbtre.com.vn", TrangThai = true }
        };
        var nxbs = await context.NhaXuatBans.ToListAsync();
        var missingNxbs = nxbSeeds
            .Where(seed => !nxbs.Any(existing =>
                string.Equals(existing.TenNhaXuatBan, seed.TenNhaXuatBan, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (missingNxbs.Count > 0)
        {
            context.NhaXuatBans.AddRange(missingNxbs);
            await context.SaveChangesAsync();
            nxbs.AddRange(missingNxbs);
        }

        // Kiểm tra nếu đã có Sách thì không seed lại
        if (!await context.Sachs.AnyAsync())
        {
            var nxb1 = nxbs.First(n =>
                string.Equals(n.TenNhaXuatBan, "NXB Giáo Dục Việt Nam", StringComparison.OrdinalIgnoreCase));
            var nxb2 = nxbs.First(n =>
                string.Equals(n.TenNhaXuatBan, "NXB Khoa Học và Kỹ Thuật", StringComparison.OrdinalIgnoreCase));
            var nxb3 = nxbs.First(n =>
                string.Equals(n.TenNhaXuatBan, "NXB Trẻ", StringComparison.OrdinalIgnoreCase));

            int defaultTheLoaiId = await context.TheLoais
                .Select(t => t.MaTheLoai)
                .FirstOrDefaultAsync();
            if (defaultTheLoaiId == 0)
            {
                throw new InvalidOperationException(
                    "Cannot seed books because no book category exists. Seed categories before seeding books.");
            }

            var sachs = new List<Sach>
            {
                new Sach 
                { 
                    TenSach = "Lập trình C# cơ bản và nâng cao", 
                    TacGia = "PGS.TS Đỗ Văn Thông", 
                    MaTheLoai = defaultTheLoaiId, 
                    MaNhaXuatBan = nxb2.MaNhaXuatBan,
                    NamXuatBan = 2024, 
                    SoLuong = 50, 
                    SoLuongCon = 50, 
                    DonGia = 120000, 
                    ViTriKeSach = "Kệ A1-01", 
                    TrangThai = TrangThaiSach.ConPhucVu, 
                    GhiChu = "Sách chuyên ngành CNTT" 
                },
                new Sach 
                { 
                    TenSach = "Cấu trúc dữ liệu và giải thuật với Java", 
                    TacGia = "ThS. Nguyễn Văn A", 
                    MaTheLoai = defaultTheLoaiId, 
                    MaNhaXuatBan = nxb1.MaNhaXuatBan,
                    NamXuatBan = 2023, 
                    SoLuong = 30, 
                    SoLuongCon = 28, 
                    DonGia = 95000, 
                    ViTriKeSach = "Kệ A1-02", 
                    TrangThai = TrangThaiSach.ConPhucVu, 
                    GhiChu = "Dùng cho học phần Lập trình Java" 
                },
                new Sach 
                { 
                    TenSach = "Nhập môn Trí tuệ nhân tạo", 
                    TacGia = "Stuart Russell, Peter Norvig", 
                    MaTheLoai = defaultTheLoaiId, 
                    MaNhaXuatBan = nxb3.MaNhaXuatBan,
                    NamXuatBan = 2022, 
                    SoLuong = 20, 
                    SoLuongCon = 20, 
                    DonGia = 250000, 
                    ViTriKeSach = "Kệ B2-05", 
                    TrangThai = TrangThaiSach.ConPhucVu, 
                    GhiChu = "Sách tham khảo quốc tế" 
                }
            };
            context.Sachs.AddRange(sachs);
            await context.SaveChangesAsync();
        }
    }
}