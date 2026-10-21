// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: C?u hình AppDbContext qu?n lý 7 b?ng Entity, c?u hình Fluent API (tránh l?i cascade delete) và Seed Data.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
    public DbSet<TheLoai> TheLoais { get; set; } = null!;
    public DbSet<NhaXuatBan> NhaXuatBans { get; set; } = null!;
    public DbSet<Sach> Sachs { get; set; } = null!;
    public DbSet<BanDoc> BanDocs { get; set; } = null!;
    public DbSet<PhieuMuon> PhieuMuons { get; set; } = null!;
    public DbSet<ChiTietMuon> ChiTietMuons { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap)
            .IsUnique();

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.Email)
            .IsUnique();

        // C?u hình tránh l?i multiple cascade paths cho PhieuMuon - NhanVien (TaiKhoan)
        modelBuilder.Entity<PhieuMuon>()
            .HasOne(p => p.NhanVien)
            .WithMany(t => t.PhieuMuonDaXuLy)
            .HasForeignKey(p => p.MaNhanVien)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaiKhoan>().HasData(
            new TaiKhoan
            {
                MaTaiKhoan = 1,
                TenDangNhap = "admin",
                MatKhau = "123456",
                HoTen = "Qu?n Tr? Viên",
                Email = "admin@uneti.edu.vn",
                VaiTro = VaiTro.Admin,
                TrangThai = true,
                NgayTao = new DateTime(2026, 1, 1)
            },
            new TaiKhoan
            {
                MaTaiKhoan = 2,
                TenDangNhap = "thuthu01",
                MatKhau = "123456",
                HoTen = "Th? Thu Viên",
                Email = "thuthu@uneti.edu.vn",
                VaiTro = VaiTro.ThuThu,
                TrangThai = true,
                NgayTao = new DateTime(2026, 1, 1)
            }
        );

        modelBuilder.Entity<TheLoai>().HasData(
            new TheLoai { MaTheLoai = 1, TenTheLoai = "Công ngh? thông tin", MoTa = "Sách chuyên ngành CNTT và Ph?n m?m", TrangThai = true, ThuTuHienThi = 1 },
            new TheLoai { MaTheLoai = 2, TenTheLoai = "Kinh t? - Qu?n tr?", MoTa = "Sách qu?n tr? kinh doanh và tài chính", TrangThai = true, ThuTuHienThi = 2 },
            new TheLoai { MaTheLoai = 3, TenTheLoai = "Ngo?i ng?", MoTa = "Sách h?c Ti?ng Anh, Ti?ng Nh?t, Ti?ng Trung", TrangThai = true, ThuTuHienThi = 3 }
        );
    }
}
