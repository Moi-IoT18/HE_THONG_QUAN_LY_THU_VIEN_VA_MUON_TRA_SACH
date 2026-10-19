// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Nghiệp vụ phía Bạn đọc: giỏ mượn (tối đa 5 cuốn), tạo phiếu sau khi
//                     kiểm tra điều kiện, hạn trả do hệ thống tính, theo dõi phiếu, hủy khi Chờ duyệt.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class PhieuMuonService : IPhieuMuonService
{
    private readonly IPhieuMuonRepository _repo;
    private readonly IRepository<Sach> _sachRepo;
    private readonly IMuonSachService _muonSach;
    private readonly IQuaHanService _quaHan;
 
    public PhieuMuonService(IPhieuMuonRepository repo, IRepository<Sach> sachRepo,
        IMuonSachService muonSach, IQuaHanService quaHan)
    {
        _repo = repo;
        _sachRepo = sachRepo;
        _muonSach = muonSach;
        _quaHan = quaHan;
    }
 
    // =====================================================================
    // GIỎ MƯỢN
    // =====================================================================
 
    public async Task<GioMuonViewModel> LayGioMuonAsync(int maBanDoc, List<int> gio)
    {
        var sachs = await _sachRepo.QueryNoTracking()
            .Include(s => s.TheLoai)
            .Where(s => gio.Contains(s.MaSach))
            .ToListAsync();
 
        var vm = new GioMuonViewModel { Sachs = sachs };
        if (sachs.Count > 0)
        {
            // Kiểm tra trước để bạn đọc thấy lý do nếu chưa đủ điều kiện
            vm.CanhBao = await _muonSach.KiemTraDieuKienAsync(maBanDoc, sachs.Select(s => new SachYeuCau(s.MaSach, 1)).ToList());
        }
        return vm;
    }
 
    public async Task<ServiceResult> ThemVaoGioAsync(int maSach, List<int> gio)
    {
        var sach = await _sachRepo.QueryNoTracking().FirstOrDefaultAsync(s => s.MaSach == maSach);
 
        if (sach is null)
            return ServiceResult.Loi("Sách không tồn tại.");
        if (!sach.KhaDung)
            return ServiceResult.Loi($"Sách \"{sach.TenSach}\" hiện không khả dụng.");
        if (gio.Contains(maSach))
            return ServiceResult.Loi($"Sách \"{sach.TenSach}\" đã có trong giỏ.");
        if (gio.Count >= QuyDinh.SoSachToiDa)
            return ServiceResult.Loi($"Giỏ mượn tối đa {QuyDinh.SoSachToiDa} cuốn.");
 
        gio.Add(maSach);
        return ServiceResult.Ok($"Đã thêm \"{sach.TenSach}\" vào giỏ mượn ({gio.Count} cuốn).");
    }
 
    // =====================================================================
    // ĐĂNG KÝ MƯỢN
    // =====================================================================
 
    public async Task<ServiceResult<int>> TaoPhieuAsync(int maBanDoc, List<int> gio, string? ghiChu)
    {
        var yeuCau = gio.Select(ma => new SachYeuCau(ma, 1)).ToList();
 
        var loi = await _muonSach.KiemTraDieuKienAsync(maBanDoc, yeuCau);
        if (loi.Count > 0)
        {
            return ServiceResult<int>.Loi("Chưa thể tạo phiếu mượn:\n• " + string.Join("\n• ", loi));
        }
 
        // Thời điểm tạo và hạn trả do hệ thống xác định
        var bayGio = DateTime.Now;
        var phieu = new PhieuMuon
        {
            MaBanDoc = maBanDoc,
            NgayMuon = bayGio,
            HanTra = bayGio.Date.AddDays(QuyDinh.SoNgayMuon),
            TrangThai = TrangThaiPhieu.ChoDuyet,
            GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim()[..Math.Min(ghiChu.Trim().Length, 500)],
            ChiTietMuons = yeuCau.Select(y => new ChiTietMuon
            {
                MaSach = y.MaSach,
                SoLuong = y.SoLuong,
                TinhTrangMuon = TinhTrangMuon.BinhThuong,
            }).ToList(),
        };
        await _repo.AddAsync(phieu);
        await _repo.SaveChangesAsync();
 
        return ServiceResult<int>.Ok(phieu.MaPhieuMuon,
            $"Đã gửi phiếu mượn #{phieu.MaPhieuMuon}. Vui lòng chờ thủ thư duyệt và đến nhận sách.");
    }
 
    // =====================================================================
    // THEO DÕI PHIẾU CỦA TÔI
    // =====================================================================
 
    public async Task<PhieuCuaToiViewModel> LayPhieuCuaToiAsync(int maBanDoc, TrangThaiPhieu? trangThai, int trang)
    {
        await _quaHan.QuetQuaHanAsync();
 
        var nguongSapHan = DateTime.Today.AddDays(QuyDinh.SoNgaySapDenHan);
        return new PhieuCuaToiViewModel
        {
            TrangThai = trangThai,
            DanhSach = await PhanTrang<PhieuMuon>.TaoAsync(_repo.TruyVanCuaBanDoc(maBanDoc, trangThai), trang, QuyDinh.KichThuocTrang),
            CanhBaoHan = await _repo.LayCanhBaoHanAsync(maBanDoc, nguongSapHan),
        };
    }
 
    public Task<PhieuMuon?> LayChiTietAsync(int maPhieu, int maBanDoc) => _repo.LayChiTietCuaBanDocAsync(maPhieu, maBanDoc);
 
    public async Task<ServiceResult> HuyAsync(int maPhieu, int maBanDoc)
    {
        var phieu = await _repo.TimCuaBanDocAsync(maPhieu, maBanDoc);
        if (phieu is null) return ServiceResult.KhongTonTai();
 
        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DaHuy))
            return ServiceResult.Loi($"Phiếu đang ở trạng thái \"{phieu.TrangThai.TenHienThi()}\" nên không thể hủy.");
 
        phieu.TrangThai = TrangThaiPhieu.DaHuy;
        phieu.GhiChu = $"Bạn đọc tự hủy lúc {DateTime.Now:dd/MM/yyyy HH:mm}";
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã hủy phiếu #{maPhieu}.");
    }
}
