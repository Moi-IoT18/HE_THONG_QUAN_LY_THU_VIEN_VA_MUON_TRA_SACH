// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Module 2 – Truy vấn LINQ / EF Core cho bảng NhaXuatBan.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public class NhaXuatBanRepository : Repository<NhaXuatBan>, INhaXuatBanRepository
{
    public NhaXuatBanRepository(AppDbContext db) : base(db) { }
 
    public IQueryable<NhaXuatBanDong> TruyVanDanhSach(string? tuKhoa, bool? trangThai)
    {
        var q = _dbSet.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var k = tuKhoa.Trim();
            q = q.Where(n => n.TenNhaXuatBan.Contains(k) || (n.DiaChi != null && n.DiaChi.Contains(k)));
        }
        if (trangThai.HasValue)
        {
            q = q.Where(n => n.TrangThai == trangThai.Value);
        }
 
        return q.OrderBy(n => n.TenNhaXuatBan).Select(n => new NhaXuatBanDong
        {
            MaNhaXuatBan = n.MaNhaXuatBan,
            TenNhaXuatBan = n.TenNhaXuatBan,
            DiaChi = n.DiaChi,
            SoDienThoai = n.SoDienThoai,
            Email = n.Email,
            TrangThai = n.TrangThai,
            SoDauSach = n.Sachs.Count(),
        });
    }
 
    public Task<NhaXuatBan?> LayChiTietAsync(int maNhaXuatBan)
        => _dbSet.AsNoTracking()
            .Include(n => n.Sachs.OrderBy(s => s.TenSach)).ThenInclude(s => s.TheLoai)
            .FirstOrDefaultAsync(n => n.MaNhaXuatBan == maNhaXuatBan);
 
    public Task<bool> TrungTenAsync(string ten, int maBoQua = 0)
    {
        var tenThuong = ten.Trim().ToLower();
        return _dbSet.AnyAsync(n => n.TenNhaXuatBan.ToLower() == tenThuong && n.MaNhaXuatBan != maBoQua);
    }
 
    public Task<int> DemSachAsync(int maNhaXuatBan) => _db.Sachs.CountAsync(s => s.MaNhaXuatBan == maNhaXuatBan);
}
