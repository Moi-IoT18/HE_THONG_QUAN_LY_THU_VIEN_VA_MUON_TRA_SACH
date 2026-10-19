// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Dashboard: các chỉ số tổng quan mục 9.3 của đề, tính bằng LINQ Count()/Sum().
//                     Chỉ đọc dữ liệu nên dùng Repository tổng quát IRepository<T>.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class DashboardService : IDashboardService
{
    private readonly IRepository<TheLoai> _theLoaiRepo;
    private readonly IRepository<Sach> _sachRepo;
    private readonly IRepository<BanDoc> _banDocRepo;
    private readonly IRepository<PhieuMuon> _phieuRepo;
    private readonly IQuaHanService _quaHan;
 
    public DashboardService(IRepository<TheLoai> theLoaiRepo, IRepository<Sach> sachRepo,
        IRepository<BanDoc> banDocRepo, IRepository<PhieuMuon> phieuRepo, IQuaHanService quaHan)
    {
        _theLoaiRepo = theLoaiRepo;
        _sachRepo = sachRepo;
        _banDocRepo = banDocRepo;
        _phieuRepo = phieuRepo;
        _quaHan = quaHan;
    }
 
    public async Task<DashboardViewModel> LayDashboardAsync()
    {
        // Cập nhật quá hạn trước để số liệu chính xác
        var vuaQuet = await _quaHan.QuetQuaHanAsync();
        var phieus = _phieuRepo.QueryNoTracking();
 
        return new DashboardViewModel
        {
            SoPhieuVuaQuetQuaHan = vuaQuet,
            TongTheLoai = await _theLoaiRepo.CountAsync(),
            TongDauSach = await _sachRepo.CountAsync(),
            TongSoCuon = await _sachRepo.QueryNoTracking().SumAsync(s => s.SoLuong),
            SoSachKhaDung = await _sachRepo.CountAsync(s => s.TrangThai == TrangThaiSach.ConPhucVu && s.SoLuongCon > 0),
            TongBanDoc = await _banDocRepo.CountAsync(),
            TongGiaoDich = await _phieuRepo.CountAsync(),
            SoChoXuLy = await _phieuRepo.CountAsync(p => p.TrangThai == TrangThaiPhieu.ChoDuyet),
            SoDangXuLy = await _phieuRepo.CountAsync(p => p.TrangThai == TrangThaiPhieu.DangMuon || p.TrangThai == TrangThaiPhieu.QuaHan),
            SoQuaHan = await _phieuRepo.CountAsync(p => p.TrangThai == TrangThaiPhieu.QuaHan),
            SoHoanThanh = await _phieuRepo.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra),
            TongTienPhat = await phieus.Where(p => p.TrangThai == TrangThaiPhieu.DaTra).SumAsync(p => p.TienPhat),
            PhieuChoDuyet = await phieus
                .Include(p => p.BanDoc).Include(p => p.ChiTietMuons)
                .Where(p => p.TrangThai == TrangThaiPhieu.ChoDuyet)
                .OrderBy(p => p.NgayMuon)
                .Take(5)
                .ToListAsync(),
            PhieuQuaHanLauNhat = await phieus
                .Include(p => p.BanDoc).Include(p => p.ChiTietMuons)
                .Where(p => p.TrangThai == TrangThaiPhieu.QuaHan)
                .OrderBy(p => p.HanTra)
                .Take(5)
                .ToListAsync(),
        };
    }
}
