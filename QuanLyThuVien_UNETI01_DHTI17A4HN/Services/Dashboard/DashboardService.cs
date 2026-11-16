// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Tổng hợp số liệu Dashboard từ SQL Server bằng EF Core.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    private readonly IThongKeService _thongKe;

    public DashboardService(AppDbContext db, IThongKeService thongKe)
    {
        _db = db;
        _thongKe = thongKe;
    }

    public async Task<DashboardViewModel> LayTongQuanAsync()
    {
        var thongKe = await _thongKe.LayThongKeAsync();

        var sachTheoTheLoai = await _db.TheLoais.CountAsync();
        var tongSoSach = await _db.Sachs.CountAsync();
        var soSachKhaDung = await _db.Sachs
            .Where(s => s.TrangThai == TrangThaiSach.ConPhucVu)
            .SumAsync(s => (int?)s.SoLuongCon) ?? 0;
        var tongSoBanDoc = await _db.BanDocs.CountAsync();
        var tongSoGiaoDich = await _db.PhieuMuons.CountAsync();
        var choXuLy = await _db.PhieuMuons.CountAsync(p => p.TrangThai == TrangThaiPhieu.ChoDuyet);
        var dangXuLy = await _db.PhieuMuons.CountAsync(p =>
            p.TrangThai == TrangThaiPhieu.DaDuyet
            || p.TrangThai == TrangThaiPhieu.DangMuon
            || p.TrangThai == TrangThaiPhieu.QuaHan);
        var hoanThanh = await _db.PhieuMuons.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra);
        var tongTienPhat = await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra)
            .SumAsync(p => (decimal?)p.TienPhat) ?? 0m;
        var tongTienPhatDaNop = await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra && p.DaNopTienPhat)
            .SumAsync(p => (decimal?)p.TienPhat) ?? 0m;
        var tongTienPhatChuaNop = await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra && !p.DaNopTienPhat)
            .SumAsync(p => (decimal?)p.TienPhat) ?? 0m;
        return new DashboardViewModel
        {
            TongSoTheLoai = sachTheoTheLoai,
            TongSoSach = tongSoSach,
            SoSachKhaDung = soSachKhaDung,
            TongSoBanDoc = tongSoBanDoc,
            TongSoGiaoDich = tongSoGiaoDich,
            SoGiaoDichChoXuLy = choXuLy,
            SoGiaoDichDangXuLy = dangXuLy,
            SoGiaoDichHoanThanh = hoanThanh,
            TongTienPhat = tongTienPhat,
            TongTienPhatDaNop = tongTienPhatDaNop,
            TongTienPhatChuaNop = tongTienPhatChuaNop,
            LuotMuonTheoThang = thongKe.LuotMuonTheoThang,
            SachTheoTheLoai = thongKe.SachTheoTheLoai,
            SachMuonNhieu = thongKe.SachMuonNhieu,
        };
    }
}
