// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Tính thống kê thư viện bằng truy vấn LINQ/EF Core trên SQL Server.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;

public sealed class ThongKeService : IThongKeService
{
    private readonly AppDbContext _db;
    private readonly IQuaHanService _quaHan;

    public ThongKeService(AppDbContext db, IQuaHanService quaHan)
    {
        _db = db;
        _quaHan = quaHan;
    }

    public async Task<ThongKeViewModel> LayThongKeAsync()
    {
        await _quaHan.QuetQuaHanAsync();

        // GroupBy thể loại cho biết số đầu sách và Sum tổng số bản vật lý trong kho.
        var sachTheoTheLoai = await _db.TheLoais.AsNoTracking()
            .OrderBy(t => t.ThuTuHienThi)
            .ThenBy(t => t.TenTheLoai)
            .Select(t => new SachTheoTheLoaiViewModel
            {
                TenTheLoai = t.TenTheLoai,
                SoDauSach = t.Sachs.Count,
                TongSoBan = t.Sachs.Sum(s => (int?)s.SoLuong) ?? 0,
            })
            .ToListAsync();

        var dauThangHienTai = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var tuThang = dauThangHienTai.AddMonths(-11);
        // Chỉ đếm phiếu đã được duyệt/đang mượn/đã trả; chờ duyệt và đã hủy không phải lượt mượn.
        var thangDaCoDuLieu = await _db.PhieuMuons.AsNoTracking()
            .Where(p => p.NgayMuon >= tuThang
                && (p.TrangThai == TrangThaiPhieu.DangMuon
                    || p.TrangThai == TrangThaiPhieu.QuaHan
                    || p.TrangThai == TrangThaiPhieu.DaTra))
            .GroupBy(p => new { p.NgayMuon.Year, p.NgayMuon.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                SoLuotMuon = g.Count(),
            })
            .ToListAsync();

        var luotMuonTheoThang = Enumerable.Range(0, 12)
            .Select(offset => tuThang.AddMonths(offset))
            .Select(thang => new LuotMuonTheoThangViewModel
            {
                Nam = thang.Year,
                Thang = thang.Month,
                SoLuotMuon = thangDaCoDuLieu
                    .Where(x => x.Year == thang.Year && x.Month == thang.Month)
                    .Select(x => x.SoLuotMuon)
                    .FirstOrDefault(),
            })
            .ToList();

        // GroupBy sách rồi Sum số lượng thể hiện tổng số bản sách đã được cho mượn.
        var sachMuonNhieu = await _db.ChiTietMuons.AsNoTracking()
            .Where(ct => ct.PhieuMuon!.TrangThai == TrangThaiPhieu.DangMuon
                || ct.PhieuMuon!.TrangThai == TrangThaiPhieu.QuaHan
                || ct.PhieuMuon!.TrangThai == TrangThaiPhieu.DaTra)
            .GroupBy(ct => new { ct.MaSach, ct.Sach!.TenSach, ct.Sach.DuongDanAnhBia })
            .Select(g => new SachMuonNhieuViewModel
            {
                MaSach = g.Key.MaSach,
                TenSach = g.Key.TenSach,
                DuongDanAnhBia = g.Key.DuongDanAnhBia,
                SoLuongMuon = g.Sum(ct => ct.SoLuong),
            })
            .OrderByDescending(x => x.SoLuongMuon)
            .ThenBy(x => x.TenSach)
            .Take(10)
            .ToListAsync();

        // GroupBy bạn đọc rồi Count phiếu hoàn thành/đang mượn để xếp hạng lượt mượn.
        var banDocMuonNhieu = await _db.PhieuMuons.AsNoTracking()
            .Where(p => p.TrangThai == TrangThaiPhieu.DangMuon
                || p.TrangThai == TrangThaiPhieu.QuaHan
                || p.TrangThai == TrangThaiPhieu.DaTra)
            .GroupBy(p => new { p.MaBanDoc, p.BanDoc!.HoTen })
            .Select(g => new BanDocMuonNhieuViewModel
            {
                HoTen = g.Key.HoTen,
                SoLuotMuon = g.Count(),
            })
            .OrderByDescending(x => x.SoLuotMuon)
            .ThenBy(x => x.HoTen)
            .Take(10)
            .ToListAsync();

        var soPhieuQuaHan = await _db.PhieuMuons.CountAsync(p =>
            p.TrangThai == TrangThaiPhieu.QuaHan
            || (p.TrangThai == TrangThaiPhieu.DangMuon && p.HanTra < DateTime.Today));
        var tongTienPhat = await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra)
            .SumAsync(p => (decimal?)p.TienPhat) ?? 0m;
        var tongTienPhatDaNop = await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra && p.DaNopTienPhat)
            .SumAsync(p => (decimal?)p.TienPhat) ?? 0m;
        var tongTienPhatChuaNop = await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra && !p.DaNopTienPhat)
            .SumAsync(p => (decimal?)p.TienPhat) ?? 0m;
        var tongPhieuDaTra = await _db.PhieuMuons.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra);
        var phieuTraDungHan = await _db.PhieuMuons.CountAsync(p =>
            p.TrangThai == TrangThaiPhieu.DaTra && p.NgayTra.HasValue && p.NgayTra.Value <= p.HanTra);

        return new ThongKeViewModel
        {
            SachTheoTheLoai = sachTheoTheLoai,
            LuotMuonTheoThang = luotMuonTheoThang,
            SachMuonNhieu = sachMuonNhieu,
            BanDocMuonNhieu = banDocMuonNhieu,
            SoPhieuQuaHan = soPhieuQuaHan,
            TongTienPhat = tongTienPhat,
            TongTienPhatDaNop = tongTienPhatDaNop,
            TongTienPhatChuaNop = tongTienPhatChuaNop,
            TongPhieuDaTra = tongPhieuDaTra,
            PhieuTraDungHan = phieuTraDungHan,
            TyLeTraDungHan = tongPhieuDaTra == 0 ? 0m : Math.Round(phieuTraDungHan * 100m / tongPhieuDaTra, 2),
        };
    }
}
