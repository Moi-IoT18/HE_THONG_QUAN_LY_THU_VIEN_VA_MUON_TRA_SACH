// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Thực hiện truy vấn EF Core cho chi tiết mượn và giao dịch liên quan.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using ChiTietMuonModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.ChiTietMuon;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.ChiTietMuon;

public sealed class ChiTietMuonRepository : IChiTietMuonRepository
{
    private readonly AppDbContext _db;

    public ChiTietMuonRepository(AppDbContext db) => _db = db;

    public IQueryable<ChiTietMuonModel> TruyVanDanhSach(string? tuKhoa, TrangThaiPhieu? trangThai)
    {
        var query = _db.ChiTietMuons.AsNoTracking()
            .Include(ct => ct.PhieuMuon).ThenInclude(p => p!.BanDoc)
            .Include(ct => ct.Sach)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = int.TryParse(keyword, out var maPhieu)
                ? query.Where(ct => ct.Sach!.TenSach.Contains(keyword)
                    || ct.PhieuMuon!.BanDoc!.HoTen.Contains(keyword)
                    || ct.MaPhieuMuon == maPhieu)
                : query.Where(ct => ct.Sach!.TenSach.Contains(keyword)
                    || ct.PhieuMuon!.BanDoc!.HoTen.Contains(keyword));
        }

        if (trangThai.HasValue)
            query = query.Where(ct => ct.PhieuMuon!.TrangThai == trangThai.Value);

        return query.OrderByDescending(ct => ct.PhieuMuon!.NgayMuon)
            .ThenBy(ct => ct.MaChiTiet);
    }

    public Task<ChiTietMuonModel?> LayChiTietAsync(int maChiTiet)
        => _db.ChiTietMuons.AsNoTracking()
            .Include(ct => ct.PhieuMuon).ThenInclude(p => p!.BanDoc)
            .Include(ct => ct.PhieuMuon).ThenInclude(p => p!.NhanVien)
            .Include(ct => ct.Sach).ThenInclude(s => s!.TheLoai)
            .FirstOrDefaultAsync(ct => ct.MaChiTiet == maChiTiet);
}
