// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: C?u hình IDesignTimeDbContextFactory h? tr? t?o Migration khi thi?t k? co s? d? li?u.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=QuanLyThuVien_UNETI01_DHTI17A4HN;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}
