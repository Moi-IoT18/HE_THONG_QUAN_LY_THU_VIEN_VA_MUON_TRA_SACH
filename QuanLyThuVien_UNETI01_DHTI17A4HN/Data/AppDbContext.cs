// Họ và tên: Nguyễn Thế Đông - Mã sinh viên: 23103100187
// Nội dung thực hiện: Khởi tạo DbContext, khai báo các DbSet và cấu hình Fluent API quan hệ CSDL.
// Bổ sung: Đặng Thị Phượng - 23103100182
// Nội dung thực hiện: Ngăn cascade delete làm mất dữ liệu thư viện và lịch sử mượn.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data
{
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

            // Cấu hình Unique constraints
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<TheLoai>()
                .HasIndex(t => t.TenTheLoai)
                .IsUnique();

            modelBuilder.Entity<BanDoc>()
                .HasOne(b => b.TaiKhoan)
                .WithOne(t => t.BanDoc)
                .HasForeignKey<BanDoc>(b => b.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sach>()
                .HasOne(s => s.TheLoai)
                .WithMany(t => t.Sachs)
                .HasForeignKey(s => s.MaTheLoai)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sach>()
                .HasOne(s => s.NhaXuatBan)
                .WithMany(n => n.Sachs)
                .HasForeignKey(s => s.MaNhaXuatBan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuMuon>()
                .HasOne(p => p.BanDoc)
                .WithMany(b => b.PhieuMuons)
                .HasForeignKey(p => p.MaBanDoc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuMuon>()
                .HasOne(p => p.NhanVienThuTienPhat)
                .WithMany()
                .HasForeignKey(p => p.NguoiThuTienPhat)
                .OnDelete(DeleteBehavior.Restrict);

            // Khóa chính cho ChiTietMuon (nếu dùng khóa tổng hợp hoặc MaChiTiet)
            modelBuilder.Entity<ChiTietMuon>()
                .HasKey(c => c.MaChiTiet);

            // Cấu hình quan hệ PhieuMuon - ChiTietMuon
            modelBuilder.Entity<ChiTietMuon>()
                .HasOne(c => c.PhieuMuon)
                .WithMany(p => p.ChiTietMuons)
                .HasForeignKey(c => c.MaPhieuMuon)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ Sach - ChiTietMuon
            modelBuilder.Entity<ChiTietMuon>()
                .HasOne(c => c.Sach)
                .WithMany(s => s.ChiTietMuons)
                .HasForeignKey(c => c.MaSach)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}