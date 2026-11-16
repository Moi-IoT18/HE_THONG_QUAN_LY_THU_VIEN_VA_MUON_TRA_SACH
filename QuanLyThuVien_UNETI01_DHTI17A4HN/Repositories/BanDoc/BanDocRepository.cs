// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Truy vấn LINQ / EF Core cho bảng BanDoc.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public class BanDocRepository : Repository<BanDoc>, IBanDocRepository
{
    public BanDocRepository(AppDbContext db) : base(db) { }
 
    public IQueryable<BanDocDong> TruyVanDanhSach(string? tuKhoa, TrangThaiBanDoc? trangThai, string? tinhTrangThe, string sapXep)
    {
        var homNay = DateTime.Today;
        var q = _dbSet.AsNoTracking();
 
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var k = tuKhoa.Trim();
            q = q.Where(b => b.HoTen.Contains(k)
                             || (b.SoDienThoai != null && b.SoDienThoai.Contains(k))
                             || (b.Email != null && b.Email.Contains(k))
                             || b.TaiKhoan!.TenDangNhap.Contains(k));
        }
        if (trangThai.HasValue)
            q = q.Where(b => b.TrangThai == trangThai.Value);
        if (tinhTrangThe == "conhan")
            q = q.Where(b => b.HanThe >= homNay);
        else if (tinhTrangThe == "hethan")
            q = q.Where(b => b.HanThe < homNay);
 
        q = sapXep switch
        {
            "ten_desc" => q.OrderByDescending(b => b.HoTen),
            "ngaycap_desc" => q.OrderByDescending(b => b.NgayCapThe),
            "hanthe_asc" => q.OrderBy(b => b.HanThe),
            _ => q.OrderBy(b => b.HoTen),
        };
 
        return q.Select(b => new BanDocDong
        {
            MaBanDoc = b.MaBanDoc,
            HoTen = b.HoTen,
            TenDangNhap = b.TaiKhoan!.TenDangNhap,
            SoDienThoai = b.SoDienThoai,
            Email = b.Email,
            NgayCapThe = b.NgayCapThe,
            HanThe = b.HanThe,
            TrangThai = b.TrangThai,
            TaiKhoanHoatDong = b.TaiKhoan.TrangThai,
            // Đếm số cuốn đang giữ bằng LINQ Sum
            SoSachDangGiu = b.PhieuMuons
                .Where(p => p.TrangThai == TrangThaiPhieu.DangMuon || p.TrangThai == TrangThaiPhieu.QuaHan)
                .SelectMany(p => p.ChiTietMuons)
                .Sum(ct => (int?)ct.SoLuong) ?? 0,
        });
    }
 
    public Task<BanDoc?> LayKemTaiKhoanAsync(int maBanDoc, bool theoDoi = false)
    {
        var q = theoDoi ? _dbSet : _dbSet.AsNoTracking();
        return q.Include(b => b.TaiKhoan).FirstOrDefaultAsync(b => b.MaBanDoc == maBanDoc);
    }
 
    public Task<BanDoc?> LayTheoTaiKhoanAsync(int? maTaiKhoan)
        => _dbSet.Include(b => b.TaiKhoan).FirstOrDefaultAsync(b => b.MaTaiKhoan == maTaiKhoan);
 
    public Task<List<PhieuMuon>> LayLichSuPhieuAsync(int maBanDoc)
        => _db.PhieuMuons.AsNoTracking()
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach)
            .Where(p => p.MaBanDoc == maBanDoc)
            .OrderByDescending(p => p.NgayMuon)
            .ToListAsync();
 
    public async Task<int> DemSachDangGiuAsync(int maBanDoc)
        => await _db.PhieuMuons
            .Where(p => p.MaBanDoc == maBanDoc
                        && (p.TrangThai == TrangThaiPhieu.DangMuon || p.TrangThai == TrangThaiPhieu.QuaHan))
            .SelectMany(p => p.ChiTietMuons)
            .SumAsync(ct => (int?)ct.SoLuong) ?? 0;
 
    public Task<int> DemPhieuAsync(int maBanDoc) => _db.PhieuMuons.CountAsync(p => p.MaBanDoc == maBanDoc);
 
    public Task<bool> TrungTenDangNhapAsync(string tenDangNhap) => _db.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap);
 
    public void XoaKemTaiKhoan(BanDoc banDoc)
    {
        _dbSet.Remove(banDoc);
        if (banDoc.TaiKhoan is not null) _db.TaiKhoans.Remove(banDoc.TaiKhoan);
    }
}
