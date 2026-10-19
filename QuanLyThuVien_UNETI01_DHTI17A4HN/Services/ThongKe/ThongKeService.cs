// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Thống kê bằng LINQ (mục 9.4 của đề) – Count(), Sum(), GroupBy(), OrderBy(),
//                     OrderByDescending(), Any(). Chỉ đọc dữ liệu nên dùng Repository tổng quát IRepository<T>.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class ThongKeService : IThongKeService
{
    private readonly IRepository<Sach> _sachRepo;
    private readonly IRepository<PhieuMuon> _phieuRepo;
    private readonly IRepository<ChiTietMuon> _chiTietRepo;
    private readonly IQuaHanService _quaHan;
 
    public ThongKeService(IRepository<Sach> sachRepo, IRepository<PhieuMuon> phieuRepo,
        IRepository<ChiTietMuon> chiTietRepo, IQuaHanService quaHan)
    {
        _sachRepo = sachRepo;
        _phieuRepo = phieuRepo;
        _chiTietRepo = chiTietRepo;
        _quaHan = quaHan;
    }
 
    public async Task<ThongKeViewModel> LayThongKeAsync(int? nam)
    {
        await _quaHan.QuetQuaHanAsync();
 
        // Phiếu "đã mượn thực sự" = đã được duyệt giao sách
        var phieuDaMuon = _phieuRepo.QueryNoTracking().Where(p =>
            p.TrangThai == TrangThaiPhieu.DangMuon
            || p.TrangThai == TrangThaiPhieu.QuaHan
            || p.TrangThai == TrangThaiPhieu.DaTra);
 
        var dsNam = await phieuDaMuon.Select(p => p.NgayMuon.Year).Distinct().OrderByDescending(y => y).ToListAsync();
        var namChon = nam ?? (dsNam.Count > 0 ? dsNam[0] : DateTime.Today.Year);
 
        var vm = new ThongKeViewModel { Nam = namChon, DsNam = dsNam };
 
        // 1. Số sách theo thể loại – GroupBy thể loại, Count đầu sách, Sum số cuốn
        vm.SachTheoTheLoai = await _sachRepo.QueryNoTracking()
            .GroupBy(s => new { s.MaTheLoai, s.TheLoai!.TenTheLoai })
            .Select(g => new ThongKeDong
            {
                Ma = g.Key.MaTheLoai,
                Ten = g.Key.TenTheLoai,
                SoLuong = g.Count(),
                GiaTri = g.Sum(s => s.SoLuong),
            })
            .OrderByDescending(x => x.SoLuong).ThenBy(x => x.Ten)
            .ToListAsync();
 
        // 2. Số lượt mượn theo tháng trong năm – GroupBy tháng
        var theoThang = await phieuDaMuon
            .Where(p => p.NgayMuon.Year == namChon)
            .GroupBy(p => p.NgayMuon.Month)
            .Select(g => new { Thang = g.Key, SoLuot = g.Count() })
            .ToListAsync();
        foreach (var t in theoThang)
        {
            vm.LuotMuonTheoThang[t.Thang - 1] = t.SoLuot;
        }
 
        // 3. Sách được mượn nhiều nhất – GroupBy sách, OrderByDescending
        vm.SachMuonNhieuNhat = await _chiTietRepo.QueryNoTracking()
            .Where(ct => ct.PhieuMuon!.TrangThai == TrangThaiPhieu.DangMuon
                         || ct.PhieuMuon.TrangThai == TrangThaiPhieu.QuaHan
                         || ct.PhieuMuon.TrangThai == TrangThaiPhieu.DaTra)
            .GroupBy(ct => new { ct.MaSach, ct.Sach!.TenSach, ct.Sach.TacGia })
            .Select(g => new ThongKeDong
            {
                Ma = g.Key.MaSach,
                Ten = g.Key.TenSach,
                MoTa = g.Key.TacGia,
                SoLuong = g.Count(),
                GiaTri = g.Sum(ct => ct.SoLuong),
            })
            .OrderByDescending(x => x.SoLuong).ThenBy(x => x.Ten)
            .Take(5)
            .ToListAsync();
 
        // 4. Bạn đọc mượn nhiều nhất – GroupBy bạn đọc
        vm.BanDocMuonNhieuNhat = await phieuDaMuon
            .GroupBy(p => new { p.MaBanDoc, p.BanDoc!.HoTen })
            .Select(g => new ThongKeDong
            {
                Ma = g.Key.MaBanDoc,
                Ten = g.Key.HoTen,
                SoLuong = g.Count(),
                GiaTri = g.Sum(p => p.TienPhat),
            })
            .OrderByDescending(x => x.SoLuong).ThenBy(x => x.Ten)
            .Take(5)
            .ToListAsync();
 
        // Sách chưa từng được mượn – dùng Any()
        vm.SachChuaTungMuon = await _sachRepo.QueryNoTracking()
            .Where(s => !s.ChiTietMuons.Any())
            .OrderBy(s => s.TenSach)
            .ToListAsync();
 
        // 5. Số phiếu quá hạn: đang quá hạn + đã trả nhưng trả trễ
        vm.SoPhieuDangQuaHan = await _phieuRepo.CountAsync(p => p.TrangThai == TrangThaiPhieu.QuaHan);
        vm.SoPhieuTraTre = await _phieuRepo.CountAsync(p =>
            p.TrangThai == TrangThaiPhieu.DaTra && p.NgayTra!.Value.Date > p.HanTra);
 
        // 6. Tổng tiền phạt – Sum, chia theo loại vi phạm (GroupBy tình trạng trả)
        vm.TongTienPhat = await _phieuRepo.QueryNoTracking()
            .Where(p => p.TrangThai == TrangThaiPhieu.DaTra).SumAsync(p => p.TienPhat);
        var phatTheoTinhTrang = await _chiTietRepo.QueryNoTracking()
            .Where(ct => ct.TinhTrangTra != null && ct.TienPhat > 0)
            .GroupBy(ct => ct.TinhTrangTra)
            .Select(g => new { TinhTrang = g.Key, SoDong = g.Count(), Tien = g.Sum(ct => ct.TienPhat) })
            .ToListAsync();
        vm.TienPhatTheoLoai = phatTheoTinhTrang
            .Select(x => new ThongKeDong
            {
                Ten = x.TinhTrang == TinhTrangTra.BinhThuong ? "Chỉ trả trễ hạn" : $"{x.TinhTrang!.Value.TenHienThi()} (kèm trễ hạn nếu có)",
                SoLuong = x.SoDong,
                GiaTri = x.Tien,
            })
            .OrderByDescending(x => x.GiaTri)
            .ToList();
 
        // 7. Tỷ lệ trả đúng hạn = số phiếu trả đúng hạn / số phiếu đã trả
        vm.SoPhieuDaTra = await _phieuRepo.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra);
        vm.SoPhieuTraDungHan = await _phieuRepo.CountAsync(p =>
            p.TrangThai == TrangThaiPhieu.DaTra && p.NgayTra!.Value.Date <= p.HanTra);
 
        return vm;
    }
}
