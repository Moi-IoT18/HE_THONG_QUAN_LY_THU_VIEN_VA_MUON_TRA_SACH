// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Xử lý quá hạn – hàm tính số ngày quá hạn dùng chung, quét chuyển
//                     phiếu Đang mượn -> Quá hạn, danh sách phiếu quá hạn kèm tiền phạt dự kiến (LINQ).
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class QuaHanService : IQuaHanService
{
    private readonly IRepository<PhieuMuon> _phieuRepo;
 
    public QuaHanService(IRepository<PhieuMuon> phieuRepo)
    {
        _phieuRepo = phieuRepo;
    }
 
    /// <summary>
    /// (Hàm static – dùng được ở View, DbSeeder, các Service khác mà không cần tạo đối tượng.)
    /// Số ngày quá hạn = max(0, ngày trả (hoặc hôm nay nếu chưa trả) - hạn trả). Chỉ tính phần ngày.
    /// </summary>
    public static int TinhSoNgayQuaHan(DateTime hanTra, DateTime? ngayTra = null)
    {
        var mocTinh = (ngayTra ?? DateTime.Now).Date;
        var soNgay = (mocTinh - hanTra.Date).Days;
        return Math.Max(0, soNgay);
    }
 
    /// <summary>
    /// Chuyển các phiếu Đang mượn đã qua hạn trả sang Quá hạn. Trả về số phiếu vừa cập nhật.
    /// Được gọi tự động khi mở Dashboard, danh sách phiếu và trang "Phiếu của tôi".
    /// </summary>
    public async Task<int> QuetQuaHanAsync()
    {
        var homNay = DateTime.Today;
        var phieuQuaHan = await _phieuRepo.Query()
            .Where(p => p.TrangThai == TrangThaiPhieu.DangMuon && p.HanTra < homNay)
            .ToListAsync();
 
        foreach (var phieu in phieuQuaHan)
        {
            phieu.TrangThai = TrangThaiPhieu.QuaHan;
        }
 
        if (phieuQuaHan.Count > 0)
        {
            await _phieuRepo.SaveChangesAsync();
        }
 
        return phieuQuaHan.Count;
    }
 
    public async Task<QuaHanIndexViewModel> LayDanhSachAsync(string? tuKhoa, int trang)
    {
        await QuetQuaHanAsync();
 
        var q = _phieuRepo.QueryNoTracking().Where(p => p.TrangThai == TrangThaiPhieu.QuaHan);
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var k = tuKhoa.Trim();
            q = q.Where(p => p.BanDoc!.HoTen.Contains(k) || (p.BanDoc.SoDienThoai != null && p.BanDoc.SoDienThoai.Contains(k)));
        }
 
        // Quá hạn lâu nhất (hạn trả sớm nhất) lên đầu
        var truyVan = q.OrderBy(p => p.HanTra).ThenBy(p => p.MaPhieuMuon).Select(p => new PhieuQuaHanDong
        {
            MaPhieuMuon = p.MaPhieuMuon,
            MaBanDoc = p.MaBanDoc,
            HoTen = p.BanDoc!.HoTen,
            SoDienThoai = p.BanDoc.SoDienThoai,
            HanTra = p.HanTra,
            SoCuon = p.ChiTietMuons.Sum(ct => ct.SoLuong),
            TenSachs = p.ChiTietMuons.Select(ct => ct.Sach!.TenSach).ToList(),
        });
 
        var tatCa = await _phieuRepo.QueryNoTracking()
            .Where(p => p.TrangThai == TrangThaiPhieu.QuaHan)
            .Select(p => new { p.HanTra, SoCuon = p.ChiTietMuons.Sum(ct => ct.SoLuong) })
            .ToListAsync();
 
        return new QuaHanIndexViewModel
        {
            TuKhoa = tuKhoa,
            DanhSach = await PhanTrang<PhieuQuaHanDong>.TaoAsync(truyVan, trang, QuyDinh.KichThuocTrang),
            TongPhieu = tatCa.Count,
            TongCuon = tatCa.Sum(x => x.SoCuon),
            TongPhatDuKien = tatCa.Sum(x => TinhSoNgayQuaHan(x.HanTra) * QuyDinh.PhatQuaHanMoiNgay * x.SoCuon),
        };
    }
}
