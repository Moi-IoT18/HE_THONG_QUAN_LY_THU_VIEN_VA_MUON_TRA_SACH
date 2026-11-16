// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Kiểm thử lọc kết hợp, sắp xếp và phân trang truy vấn sách EF Core.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;

namespace QuanLyThuVien.Tests;

public sealed class BookQueryTests
{
    [Fact]
    public void TruyVanDanhSach_CombinesFiltersAndPagination_InGeneratedSql()
    {
        using var db = TaoDbContext();
        var repository = new SachRepository(db);

        var sql = repository.TruyVanDanhSach(
                "C#", 8, 2024, TrangThaiSach.ConPhucVu, true, 50m, 200m, "gia_desc")
            .Skip(10)
            .Take(10)
            .ToQueryString();

        Assert.Contains("WHERE", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("LIKE", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("TacGia", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("MaTheLoai", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("NamXuatBan", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("DonGia", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SoLuongCon", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("OFFSET", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("FETCH NEXT", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ORDER BY", sql, StringComparison.OrdinalIgnoreCase);
    }

    private static AppDbContext TaoDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(local);Database=QueryTranslationOnly;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        return new AppDbContext(options);
    }
}
