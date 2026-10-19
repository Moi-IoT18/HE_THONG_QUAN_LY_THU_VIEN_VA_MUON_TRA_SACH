// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Module 4 – Truy v?n LINQ / EF Core cho ch?c nang x? lý phi?u mu?n.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

public class XuLyPhieuRepository : Repository<PhieuMuon>, IXuLyPhieuRepository
{
    public XuLyPhieuRepository(AppDbContext db) : base(db) { }

    public IQueryable<PhieuMuon> TruyVanDanhSach(PhieuMuonLocViewModel loc)
    {
        IQueryable<PhieuMuon> q = _dbSet.AsNoTracking()
            .Include(p => p.BanDoc)
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach);

        if (!string.IsNullOrWhiteSpace(loc.BanDoc))
        {
            var k = loc.BanDoc.Trim().TrimStart('#');
            if (int.TryParse(k, out var maPhieu))
                q = q.Where(p => p.MaPhieuMuon == maPhieu || p.BanDoc!.SoDienThoai!.Contains(k));
            else
                q = q.Where(p => p.BanDoc!.HoTen.Contains(k));
        }
        if (!string.IsNullOrWhiteSpace(loc.TenSach))
        {
            var k = loc.TenSach.Trim();
            q = q.Where(p => p.ChiTietMuons.Any(ct => ct.Sach!.TenSach.Contains(k)));
        }
        if (loc.MaTheLoai.HasValue)
            q = q.Where(p => p.ChiTietMuons.Any(ct => ct.Sach!.MaTheLoai == loc.MaTheLoai.Value));
        if (loc.TrangThai.HasValue)
            q = q.Where(p => p.TrangThai == loc.TrangThai.Value);
        if (loc.TuNgay.HasValue)
            q = q.Where(p => p.NgayMuon >= loc.TuNgay.Value.Date);
        if (loc.DenNgay.HasValue)
            q = q.Where(p => p.NgayMuon < loc.DenNgay.Value.Date.AddDays(1));

        return loc.SapXep switch
        {
            "ngay_asc" => q.OrderBy(p => p.NgayMuon),
            "ten_asc" => q.OrderBy(p => p.BanDoc!.HoTen).ThenByDescending(p => p.NgayMuon),
            "ten_desc" => q.OrderByDescending(p => p.BanDoc!.HoTen).ThenByDescending(p => p.NgayMuon),
            "hantra_asc" => q.OrderBy(p => p.HanTra).ThenBy(p => p.MaPhieuMuon),
            _ => q.OrderByDescending(p => p.NgayMuon),
        };
    }

    public async Task<Dictionary<TrangThaiPhieu, int>> DemTheoTrangThaiAsync()
    {
        var dem = await _dbSet
            .GroupBy(p => p.TrangThai)
            .Select(g => new { g.Key, SoLuong = g.Count() })
            .ToListAsync();
        return dem.ToDictionary(x => x.Key, x => x.SoLuong);
    }

    public Task<PhieuMuon?> LayDayDuAsync(int maPhieu, bool theoDoi)
    {
        IQueryable<PhieuMuon> q = _dbSet
            .Include(p => p.BanDoc).ThenInclude(b => b!.TaiKhoan)
            .Include(p => p.NhanVien)
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach);
        if (!theoDoi) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(p => p.MaPhieuMuon == maPhieu);
    }

    public Task<List<TheLoai>> LayTheLoaiAsync()
        => _db.TheLoais.AsNoTracking().OrderBy(t => t.ThuTuHienThi).ToListAsync();
}
