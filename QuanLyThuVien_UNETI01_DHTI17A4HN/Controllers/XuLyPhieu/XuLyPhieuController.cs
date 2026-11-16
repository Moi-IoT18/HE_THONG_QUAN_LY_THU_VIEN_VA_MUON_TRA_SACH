// Họ và tên: Trịnh Thị Thanh Tâm
// Mã sinh viên: 23103100193
// Nội dung thực hiện: Tiếp nhận, tìm kiếm, duyệt, từ chối và nhận trả phiếu mượn (Module 4).

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TheLoai;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.XuLyPhieu;

[SessionAuthorize(Roles = Quyen.NhanVienVaQuanLy)]
public class XuLyPhieuController : Controller
{
    private readonly IPhieuMuonService _phieuMuonService;
    private readonly ITheLoaiService _theLoaiService;
    private readonly INhaXuatBanService _nhaXuatBanService;
    private readonly IEmailNotificationService _emailNotificationService;

    public XuLyPhieuController(IPhieuMuonService phieuMuonService, ITheLoaiService theLoaiService,
        INhaXuatBanService nhaXuatBanService, IEmailNotificationService emailNotificationService)
    {
        _phieuMuonService = phieuMuonService;
        _theLoaiService = theLoaiService;
        _nhaXuatBanService = nhaXuatBanService;
        _emailNotificationService = emailNotificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] XuLyPhieuFilterViewModel filter)
    {
        if (filter.TuNgay.HasValue && filter.DenNgay.HasValue && filter.TuNgay.Value.Date > filter.DenNgay.Value.Date)
            ModelState.AddModelError(nameof(filter.TuNgay), "Ngày bắt đầu không được sau ngày kết thúc.");
        if (!ModelState.IsValid)
        {
            filter.TuNgay = null;
            filter.DenNgay = null;
        }

        await NapTheLoaiAsync(filter.MaTheLoai);
        await NapNhaXuatBanAsync(filter.MaNhaXuatBan);
        return View(await _phieuMuonService.LayDanhSachXuLyAsync(filter));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var phieu = await _phieuMuonService.LayChiTietXuLyAsync(id);
        return phieu is null ? NotFound() : View(phieu);
    }

    [HttpGet]
    public async Task<IActionResult> Duyet(int id)
    {
        var phieu = await _phieuMuonService.LayChiTietXuLyAsync(id);
        if (phieu is null) return NotFound();
        if (phieu.TrangThai != TrangThaiPhieu.ChoDuyet)
        {
            TempData["Loi"] = "Chỉ phiếu đang chờ duyệt mới được tiếp nhận.";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(phieu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DuyetXacNhan(int id)
    {
        var maNhanVien = HttpContext.Session.LayMaTaiKhoan();
        if (!maNhanVien.HasValue) return RedirectToAction("Login", "Auth");

        var result = await _phieuMuonService.DuyetAsync(id, maNhanVien.Value);
        TempData[result.ThanhCong ? "ThanhCong" : "Loi"] = result.ThanhCong ? result.ThongBao : result.LoiGop;
        if (result.ThanhCong)
            await GuiThongBaoAsync(id, "Phiếu mượn đã được duyệt",
                $"Phiếu mượn #{id} đã được duyệt. Vui lòng xem hạn trả trong tài khoản thư viện.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> TuChoi(int id)
    {
        var phieu = await _phieuMuonService.LayChiTietXuLyAsync(id);
        if (phieu is null) return NotFound();
        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DaHuy))
        {
            TempData["Loi"] = "Chỉ phiếu đang chờ duyệt mới được từ chối.";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(phieu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(int id, string? lyDo)
    {
        var maNhanVien = HttpContext.Session.LayMaTaiKhoan();
        if (!maNhanVien.HasValue) return RedirectToAction("Login", "Auth");

        var result = await _phieuMuonService.TuChoiAsync(id, maNhanVien.Value, lyDo ?? "");
        TempData[result.ThanhCong ? "ThanhCong" : "Loi"] = result.ThanhCong ? result.ThongBao : result.LoiGop;
        if (result.ThanhCong)
            await GuiThongBaoAsync(id, "Phiếu mượn đã bị từ chối",
                $"Phiếu mượn #{id} đã bị từ chối. Lý do: {lyDo?.Trim()}");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> TraSach(int id)
    {
        var model = await _phieuMuonService.LayFormTraSachAsync(id);
        if (model is null) return NotFound();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TraSach(int id, TraSachViewModel model)
    {
        if (id != model.MaPhieuMuon) return BadRequest();
        var maNhanVien = HttpContext.Session.LayMaTaiKhoan();
        if (!maNhanVien.HasValue) return RedirectToAction("Login", "Auth");

        if (ModelState.IsValid)
        {
            var result = await _phieuMuonService.TraSachAsync(id, maNhanVien.Value, model);
            if (result.ThanhCong)
            {
                TempData["ThanhCong"] = result.ThongBao;
                await GuiThongBaoAsync(id, "Sách đã được nhận trả",
                    $"Phiếu mượn #{id} đã được xác nhận trả sách. Tiền phạt ghi nhận: {result.ThongBao}");
                return RedirectToAction(nameof(Details), new { id });
            }
            ModelState.AddModelError(string.Empty, result.LoiGop);
        }

        var formGoc = await _phieuMuonService.LayFormTraSachAsync(id);
        if (formGoc is null) return NotFound();
        var tinhTrangTheoMa = model.ChiTiet
            .GroupBy(d => d.MaChiTiet)
            .ToDictionary(g => g.Key, g => g.First().TinhTrangTra);
        foreach (var dong in formGoc.ChiTiet)
        {
            if (tinhTrangTheoMa.TryGetValue(dong.MaChiTiet, out var tinhTrang))
                dong.TinhTrangTra = tinhTrang;
        }
        return View(formGoc);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhanNopTienPhat(int id, HinhThucNopPhat hinhThuc)
    {
        var maNhanVien = HttpContext.Session.LayMaTaiKhoan();
        if (!maNhanVien.HasValue) return RedirectToAction("Login", "Auth");

        var result = await _phieuMuonService.GhiNhanNopTienPhatAsync(id, maNhanVien.Value, hinhThuc);
        TempData[result.ThanhCong ? "ThanhCong" : "Loi"] = result.ThanhCong ? result.ThongBao : result.LoiGop;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task GuiThongBaoAsync(int maPhieu, string subject, string body)
    {
        var phieu = await _phieuMuonService.LayChiTietXuLyAsync(maPhieu);
        var email = phieu?.BanDoc?.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["CanhBao"] = "Giao dịch đã cập nhật nhưng bạn đọc chưa có email để nhận thông báo.";
            return;
        }

        var result = await _emailNotificationService.SendAsync(email, $"Thư viện UNETI: {subject}", body);
        if (result.Status != EmailDeliveryStatus.Sent)
            TempData["CanhBao"] = result.Message;
    }

    private async Task NapTheLoaiAsync(int? maTheLoai)
    {
        var danhSach = await _theLoaiService.GetAllAsync();
        ViewBag.TheLoais = new SelectList(danhSach, "MaTheLoai", "TenTheLoai", maTheLoai);
    }

    private async Task NapNhaXuatBanAsync(int? maNhaXuatBan)
    {
        var danhSach = await _nhaXuatBanService.GetAllAsync();
        ViewBag.NhaXuatBans = new SelectList(danhSach, "MaNhaXuatBan", "TenNhaXuatBan", maNhaXuatBan);
    }
}
