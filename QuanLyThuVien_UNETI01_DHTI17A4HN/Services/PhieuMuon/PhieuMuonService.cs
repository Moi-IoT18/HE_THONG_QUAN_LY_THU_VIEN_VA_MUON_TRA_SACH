// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Nghiệp vụ phía Bạn đọc: giỏ mượn (tối đa 5 cuốn), tạo phiếu sau khi
//                     kiểm tra điều kiện, hạn trả do hệ thống tính, theo dõi phiếu, hủy khi Chờ duyệt.
// Bổ sung Module 4: Trịnh Thị Thanh Tâm - 23103100193
// Nội dung thực hiện: Duyệt/từ chối/nhận trả phiếu và cập nhật tồn kho, tiền phạt có transaction.
// Bổ sung Module 5: Phan Vũ Thanh Thư - 23103100316
// Nội dung thực hiện: Lưu ghi chú xác nhận kết quả trên từng dòng chi tiết khi nhận trả.
 
using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using SachModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Sach;
using QuaHanHelper = QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers.QuaHanService;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class PhieuMuonService : IPhieuMuonService
{
    private readonly IPhieuMuonRepository _repo;
    private readonly IRepository<SachModel> _sachRepo;
    private readonly IMuonSachService _muonSach;
    private readonly IQuaHanService _quaHan;
    private readonly AppDbContext _db;
 
    public PhieuMuonService(IPhieuMuonRepository repo, IRepository<SachModel> sachRepo,
        IMuonSachService muonSach, IQuaHanService quaHan, AppDbContext db)
    {
        _repo = repo;
        _sachRepo = sachRepo;
        _muonSach = muonSach;
        _quaHan = quaHan;
        _db = db;
    }

    public async Task<XuLyPhieuFilterViewModel> LayDanhSachXuLyAsync(XuLyPhieuFilterViewModel filter)
    {
        await _quaHan.QuetQuaHanAsync();
        filter.TuKhoa = filter.TuKhoa?.Trim();
        filter.SapXep = filter.SapXep is "ngay_asc" or "ten_asc" or "ten_desc" ? filter.SapXep : "ngay_desc";
        filter.DanhSach = await PhanTrang<PhieuMuon>.TaoAsync(
            _repo.TruyVanXuLy(filter.TuKhoa, filter.MaTheLoai, filter.MaNhaXuatBan, filter.TrangThai,
                filter.TuNgay, filter.DenNgay, filter.SapXep),
            filter.Trang, QuyDinh.KichThuocTrang);
        filter.Trang = filter.DanhSach.TrangHienTai;
        return filter;
    }

    public async Task<PhieuMuon?> LayChiTietXuLyAsync(int maPhieu)
    {
        await _quaHan.QuetQuaHanAsync();
        return await _repo.LayChiTietXuLyAsync(maPhieu);
    }

    public async Task<ServiceResult> DuyetAsync(int maPhieu, int maNhanVien)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var phieu = await _repo.LayChiTietXuLyAsync(maPhieu, theoDoi: true);
        if (phieu is null) return ServiceResult.KhongTonTai();
        if (phieu.TrangThai != TrangThaiPhieu.ChoDuyet)
            return ServiceResult.Loi("Chỉ phiếu đang chờ duyệt mới được tiếp nhận.");
        if (phieu.ChiTietMuons.Count == 0)
            return ServiceResult.Loi("Phiếu không có sách để duyệt.");

        var yeuCau = phieu.ChiTietMuons.Select(ct => new SachYeuCau(ct.MaSach, ct.SoLuong)).ToList();
        var canhBao = await _muonSach.KiemTraDieuKienAsync(phieu.MaBanDoc, yeuCau, maPhieu);
        if (canhBao.Count > 0)
            return ServiceResult.Loi("Không thể duyệt phiếu:\n• " + string.Join("\n• ", canhBao));

        var maSachs = yeuCau.Select(y => y.MaSach).Distinct().ToList();
        var sachs = await _sachRepo.Query()
            .Where(s => maSachs.Contains(s.MaSach))
            .ToDictionaryAsync(s => s.MaSach);
        if (sachs.Count != maSachs.Count)
            return ServiceResult.Loi("Một hoặc nhiều sách trong phiếu không còn tồn tại.");
        foreach (var dong in phieu.ChiTietMuons)
        {
            var sach = sachs[dong.MaSach];
            if (!sach.KhaDung || sach.SoLuongCon < dong.SoLuong)
                return ServiceResult.Loi($"Sách \"{sach.TenSach}\" không còn đủ số lượng để duyệt.");
        }

        foreach (var dong in phieu.ChiTietMuons)
            sachs[dong.MaSach].SoLuongCon -= dong.SoLuong;

        var bayGio = DateTime.Now;
        phieu.NgayMuon = bayGio;
        phieu.HanTra = bayGio.Date.AddDays(QuyDinh.SoNgayMuon);
        phieu.TrangThai = TrangThaiPhieu.DangMuon;
        phieu.NhanVienXuLy = maNhanVien;
        phieu.NgayXuLy = bayGio;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return ServiceResult.Ok($"Đã duyệt phiếu #{maPhieu}; tồn kho đã được cập nhật.");
    }

    public async Task<ServiceResult> TuChoiAsync(int maPhieu, int maNhanVien, string? lyDo)
    {
        if (string.IsNullOrWhiteSpace(lyDo))
            return ServiceResult.Loi("Vui lòng nhập lý do từ chối.");
        var lyDoDaChuanHoa = lyDo.Trim();
        if (lyDoDaChuanHoa.Length is 0 or > 450)
            return ServiceResult.Loi("Lý do từ chối bắt buộc và tối đa 450 ký tự.");

        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var phieu = await _repo.LayChiTietXuLyAsync(maPhieu, theoDoi: true);
        if (phieu is null) return ServiceResult.KhongTonTai();
        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DaHuy))
            return ServiceResult.Loi("Chỉ phiếu đang chờ duyệt mới được từ chối.");

        phieu.TrangThai = TrangThaiPhieu.DaHuy;
        phieu.NhanVienXuLy = maNhanVien;
        phieu.NgayXuLy = DateTime.Now;
        phieu.GhiChu = $"Từ chối: {lyDoDaChuanHoa}";
        await _repo.SaveChangesAsync();
        await transaction.CommitAsync();
        return ServiceResult.Ok($"Đã từ chối phiếu #{maPhieu}.");
    }

    public async Task<TraSachViewModel?> LayFormTraSachAsync(int maPhieu)
    {
        await _quaHan.QuetQuaHanAsync();
        var phieu = await _repo.LayChiTietXuLyAsync(maPhieu);
        if (phieu is null || phieu.TrangThai is not (TrangThaiPhieu.DangMuon or TrangThaiPhieu.QuaHan))
            return null;

        return new TraSachViewModel
        {
            MaPhieuMuon = phieu.MaPhieuMuon,
            HoTenBanDoc = phieu.BanDoc?.HoTen ?? "",
            HanTra = phieu.HanTra,
            ChiTiet = phieu.ChiTietMuons.Select(ct => new TraSachDongViewModel
            {
                MaChiTiet = ct.MaChiTiet,
                TenSach = ct.Sach?.TenSach ?? $"Sách #{ct.MaSach}",
                DuongDanAnhBia = ct.Sach?.DuongDanAnhBia,
                SoLuong = ct.SoLuong,
                TinhTrangTra = ct.TinhTrangTra,
            }).ToList(),
            SoNgayQuaHan = QuaHanHelper.TinhSoNgayQuaHan(phieu.HanTra),
            TienPhatTamTinh = phieu.ChiTietMuons.Sum(ct =>
                QuaHanHelper.TinhTienPhatTamTinh(phieu.HanTra, ct.SoLuong)),
        };
    }

    public async Task<ServiceResult> TraSachAsync(int maPhieu, int maNhanVien, TraSachViewModel model)
    {
        if (model.ChiTiet is null)
            return ServiceResult.Loi("Danh sách sách trả không hợp lệ.");
        if (model.ChiTiet.Any(ct => ct.GhiChu?.Length > 500))
            return ServiceResult.Loi("Ghi chú tình trạng sách tối đa 500 ký tự.");
        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var phieu = await _repo.LayChiTietXuLyAsync(maPhieu, theoDoi: true);
        if (phieu is null) return ServiceResult.KhongTonTai();
        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DaTra))
            return ServiceResult.Loi("Chỉ phiếu đang mượn hoặc quá hạn mới được nhận trả.");
        if (phieu.ChiTietMuons.Count == 0 || model.ChiTiet.Count != phieu.ChiTietMuons.Count
            || model.ChiTiet.Select(x => x.MaChiTiet).Distinct().Count() != model.ChiTiet.Count)
            return ServiceResult.Loi("Danh sách sách trả không khớp với phiếu mượn.");

        var formTheoMa = model.ChiTiet.ToDictionary(x => x.MaChiTiet);
        var dongPhieu = phieu.ChiTietMuons.ToList();
        if (dongPhieu.Any(ct => !formTheoMa.ContainsKey(ct.MaChiTiet)
            || !formTheoMa[ct.MaChiTiet].TinhTrangTra.HasValue
            || !Enum.IsDefined(formTheoMa[ct.MaChiTiet].TinhTrangTra!.Value)
            || formTheoMa[ct.MaChiTiet].TinhTrangTra == TinhTrangTra.ChuaTra))
            return ServiceResult.Loi("Vui lòng chọn tình trạng hợp lệ cho từng sách.");
        if (dongPhieu.Any(ct => ct.TinhTrangTra.HasValue))
            return ServiceResult.Loi("Phiếu đã có thông tin trả sách, không thể ghi nhận lần nữa.");

        var maSachs = dongPhieu.Select(ct => ct.MaSach).Distinct().ToList();
        var sachs = await _sachRepo.Query()
            .Where(s => maSachs.Contains(s.MaSach))
            .ToDictionaryAsync(s => s.MaSach);
        if (sachs.Count != maSachs.Count)
            return ServiceResult.Loi("Một hoặc nhiều sách trong phiếu không còn tồn tại.");

        var ngayTra = DateTime.Now;
        var phatMoi = 0m;
        foreach (var dong in dongPhieu)
        {
            var sach = sachs[dong.MaSach];
            var tinhTrangTra = formTheoMa[dong.MaChiTiet].TinhTrangTra!.Value;
            var soNgayQuaHan = Math.Max(0, (ngayTra.Date - phieu.HanTra.Date).Days);
            if (soNgayQuaHan > 36500)
                return ServiceResult.Loi("Số ngày quá hạn vượt giới hạn dữ liệu cho phép.");
            var tienPhatDong = soNgayQuaHan * QuyDinh.PhatQuaHanMoiNgay * dong.SoLuong;
            if (tienPhatDong > 1_000_000_000_000m)
                return ServiceResult.Loi("Tiền phạt vượt giới hạn dữ liệu cho phép.");

            if (tinhTrangTra == TinhTrangTra.BinhThuong)
            {
                if (sach.SoLuongCon + dong.SoLuong > sach.SoLuong)
                    return ServiceResult.Loi($"Tồn kho sách \"{sach.TenSach}\" không hợp lệ; không thể nhận trả an toàn.");
                sach.SoLuongCon += dong.SoLuong;
            }
            else
            {
                if (sach.SoLuong - dong.SoLuong < sach.SoLuongCon)
                    return ServiceResult.Loi($"Tổng tồn kho sách \"{sach.TenSach}\" không đủ để ghi nhận sách hỏng/mất.");
                sach.SoLuong -= dong.SoLuong;
            }

            dong.TinhTrangTra = tinhTrangTra;
            dong.SoNgayQuaHan = soNgayQuaHan;
            dong.TienPhat = tienPhatDong;
            dong.GhiChu = string.IsNullOrWhiteSpace(formTheoMa[dong.MaChiTiet].GhiChu)
                ? null
                : formTheoMa[dong.MaChiTiet].GhiChu!.Trim();
            phatMoi += dong.TienPhat;
        }

        phieu.TrangThai = TrangThaiPhieu.DaTra;
        phieu.NgayTra = ngayTra;
        phieu.NhanVienXuLy = maNhanVien;
        phieu.NgayXuLy = ngayTra;
        phieu.TienPhat = phatMoi;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return ServiceResult.Ok($"Đã nhận trả phiếu #{maPhieu}. Tiền phạt quá hạn: {phatMoi:N0} đ.");
    }

    public async Task<ServiceResult> GhiNhanNopTienPhatAsync(
        int maPhieu, int maNhanVien, HinhThucNopPhat hinhThuc)
    {
        if (!Enum.IsDefined(hinhThuc))
            return ServiceResult.Loi("Hình thức nộp phạt không hợp lệ.");

        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var phieu = await _repo.LayChiTietXuLyAsync(maPhieu, theoDoi: true);
        if (phieu is null) return ServiceResult.KhongTonTai();
        if (phieu.TrangThai != TrangThaiPhieu.DaTra)
            return ServiceResult.Loi("Chỉ ghi nhận nộp phạt sau khi đã nhận trả sách.");
        if (phieu.TienPhat <= 0)
            return ServiceResult.Loi("Phiếu này không có khoản phạt cần thu.");
        if (phieu.DaNopTienPhat)
            return ServiceResult.Loi("Khoản phạt của phiếu này đã được ghi nhận thanh toán.");

        phieu.DaNopTienPhat = true;
        phieu.NgayNopTienPhat = DateTime.Now;
        phieu.HinhThucNopPhat = hinhThuc;
        phieu.NguoiThuTienPhat = maNhanVien;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return ServiceResult.Ok(
            $"Đã ghi nhận thu đủ {phieu.TienPhat:N0} đ tiền phạt phiếu #{maPhieu} ({hinhThuc.TenHienThi()}).");
    }
 
    // =====================================================================
    // GIỎ MƯỢN
    // =====================================================================
 
    public async Task<GioMuonViewModel> LayGioMuonAsync(int maBanDoc, List<int> gio)
    {
        var sachs = await _sachRepo.QueryNoTracking()
            .Include(s => s.TheLoai)
            .Include(s => s.NhaXuatBan)
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
        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
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
        await transaction.CommitAsync();
 
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
