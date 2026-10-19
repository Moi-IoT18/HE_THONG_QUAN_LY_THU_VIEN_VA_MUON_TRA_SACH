// H? v� t�n: Tr?nh Th? Thanh T�m
// M� sinh vi�n: 23103100193
// N?i dung th?c hi?n: Module 4 � Ti?p nh?n & x? l� phi?u mu?n (Th? thu/Admin)

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;

[Authorize(Roles = Quyen.NhanVien)]
public class XuLyPhieuController : Controller
{
    private readonly IXuLyPhieuService _service;
    public XuLyPhieuController(IXuLyPhieuService service) { _service = service; }
    private int? MaNhanVien => HttpContext.Session.LayMaTaiKhoan();

    public async Task<IActionResult> Index([FromQuery] PhieuMuonLocViewModel loc) => View(await _service.LayDanhSachAsync(loc));

    public async Task<IActionResult> ChiTiet(int id)
    {
        var (phieu, canhBao) = await _service.LayChiTietAsync(id);
        if (phieu is null) return NotFound();
        if (canhBao is not null) ViewBag.CanhBao = canhBao;
        return View(phieu);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duyet(int id)
    {
        var kq = await _service.DuyetAsync(id, MaNhanVien);
        if (kq.KhongTimThay) return NotFound();
        TempData[kq.ThanhCong ? "ThanhCong" : "Loi"] = kq.ThanhCong ? kq.ThongBao : kq.LoiGop;
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(int id, string? lyDo)
    {
        var kq = await _service.TuChoiAsync(id, lyDo, MaNhanVien);
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong) { TempData["Loi"] = kq.LoiGop; return RedirectToAction(nameof(ChiTiet), new { id }); }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> TraSach(int id)
    {
        var kq = await _service.LayFormTraSachAsync(id);
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong) { TempData["Loi"] = kq.LoiGop; return RedirectToAction(nameof(ChiTiet), new { id }); }
        return View(kq.DuLieu);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> TraSach(int id, TraSachViewModel vm)
    {
        if (id != vm.MaPhieuMuon) return BadRequest();
        if (!ModelState.IsValid)
        {
            var form = await _service.LayFormTraSachAsync(id);
            if (form.KhongTimThay) return NotFound();
            if (!form.ThanhCong) { TempData["Loi"] = form.LoiGop; return RedirectToAction(nameof(ChiTiet), new { id }); }
            form.DuLieu!.GhiChu = vm.GhiChu;
            return View(form.DuLieu);
        }
        var kq = await _service.TraSachAsync(id, vm, MaNhanVien, HttpContext.Session.LayHoTen());
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong)
        {
            if (kq.DuLieu is not null) { ModelState.ThemLoiService(kq); return View(kq.DuLieu); }
            TempData["Loi"] = kq.LoiGop;
            return RedirectToAction(nameof(ChiTiet), new { id });
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(ChiTiet), new { id });
    }
}
