// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Phía Bạn đọc: giỏ mượn, đăng ký mượn, hủy phiếu khi Chờ duyệt,
//                     theo dõi phiếu mượn của chính mình.
//                     Mọi truy vấn đều lọc theo MaBanDoc trong Session -> đổi mã trên URL không xem được phiếu người khác.
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.BanDoc)]
public class PhieuMuonController : Controller
{
    private readonly IPhieuMuonService _service;
 
    public PhieuMuonController(IPhieuMuonService service)
    {
        _service = service;
    }
 
    private int MaBanDoc => HttpContext.Session.LayMaBanDoc() ?? 0;
 
    // =====================================================================
    // GIỎ MƯỢN
    // =====================================================================
 
    public async Task<IActionResult> GioMuon()
        => View(await _service.LayGioMuonAsync(MaBanDoc, GioMuonHelper.Lay(HttpContext.Session)));
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemVaoGio(int maSach, string? returnUrl)
    {
        var gio = GioMuonHelper.Lay(HttpContext.Session);
        var kq = await _service.ThemVaoGioAsync(maSach, gio);
        if (kq.ThanhCong)
        {
            GioMuonHelper.Luu(HttpContext.Session, gio);
            TempData["ThanhCong"] = kq.ThongBao;
        }
        else
        {
            TempData["Loi"] = kq.LoiGop;
        }
 
        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(nameof(GioMuon));
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult XoaKhoiGio(int maSach)
    {
        var gio = GioMuonHelper.Lay(HttpContext.Session);
        gio.Remove(maSach);
        GioMuonHelper.Luu(HttpContext.Session, gio);
        return RedirectToAction(nameof(GioMuon));
    }
 
    /// <summary>Đăng ký mượn: tạo phiếu "Chờ duyệt" từ các sách trong giỏ.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaoPhieu(string? ghiChu)
    {
        var kq = await _service.TaoPhieuAsync(MaBanDoc, GioMuonHelper.Lay(HttpContext.Session), ghiChu);
        if (!kq.ThanhCong)
        {
            TempData["Loi"] = kq.LoiGop;
            return RedirectToAction(nameof(GioMuon));
        }
 
        GioMuonHelper.Xoa(HttpContext.Session);
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(ChiTiet), new { id = kq.DuLieu });
    }
 
    // =====================================================================
    // THEO DÕI PHIẾU MƯỢN CỦA TÔI
    // =====================================================================
 
    public async Task<IActionResult> CuaToi(TrangThaiPhieu? trangThai, int trang = 1)
        => View(await _service.LayPhieuCuaToiAsync(MaBanDoc, trangThai, trang));
 
    public async Task<IActionResult> ChiTiet(int id)
    {
        var phieu = await _service.LayChiTietAsync(id, MaBanDoc);
        if (phieu is null)
        {
            TempData["Loi"] = $"Không tìm thấy phiếu #{id} trong danh sách phiếu của bạn.";
            return RedirectToAction(nameof(CuaToi));
        }
        return View(phieu);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        var kq = await _service.HuyAsync(id, MaBanDoc);
        if (kq.KhongTimThay)
        {
            TempData["Loi"] = "Không tìm thấy phiếu mượn.";
            return RedirectToAction(nameof(CuaToi));
        }
        if (!kq.ThanhCong)
        {
            TempData["Loi"] = kq.LoiGop;
            return RedirectToAction(nameof(ChiTiet), new { id });
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(CuaToi));
    }
}
