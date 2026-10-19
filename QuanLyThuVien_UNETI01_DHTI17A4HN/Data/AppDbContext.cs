using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<NhaXuatBan> NhaXuatBans { get; set; } = null!;
    public DbSet<Sach> Sachs { get; set; } = null!;
    public DbSet<TheLoai> TheLoais { get; set; } = null!;
    public DbSet<ChiTietMuon> ChiTietMuons { get; set; } = null!;
    public DbSet<BanDoc> BanDocs { get; set; } = null!;
    public DbSet<PhieuMuon> PhieuMuons { get; set; } = null!;
    public DbSet<User> TaiKhoans => Users;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin", Description = "Quan tri vien he thong" },
            new Role { RoleId = 2, RoleName = "ThuThu", Description = "Nhan vien thu vien" },
            new Role { RoleId = 3, RoleName = "BanDoc", Description = "Doc gia / Sinh vien" }
        );

        modelBuilder.Entity<BanDoc>()
            .HasOne(b => b.TaiKhoan)
            .WithOne()
            .HasForeignKey<BanDoc>(b => b.MaTaiKhoan)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PhieuMuon>()
            .HasOne(p => p.NhanVien)
            .WithMany()
            .HasForeignKey(p => p.NhanVienXuLy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class ApplicationDbContext : AppDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
