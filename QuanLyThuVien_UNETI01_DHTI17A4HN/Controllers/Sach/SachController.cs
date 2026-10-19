// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Module 2 – Sách: tra cứu (mọi người) và quản lý (Thủ thư/Admin).
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.NhanVien)]
public class SachController : Controller
{
    private readonly ISachService _service;
 
    public SachController(ISachService service)
    {
        _service = service;
    }
 
    private List<int> SachTrongGio => GioMuonHelper.Lay(HttpContext.Session);
 
    [AllowAnonymous]
    public async Task<IActionResult> Index([FromQuery] SachLocViewModel loc)
        => View(await _service.LayDanhSachAsync(loc, SachTrongGio));
 
    public async Task<IActionResult> QuanLy([FromQuery] SachLocViewModel loc)
        => View(await _service.LayDanhSachAsync(loc, SachTrongGio));
 
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var vm = await _service.LayChiTietAsync(id, User.IsInRole(Quyen.Admin) || User.IsInRole(Quyen.ThuThu), SachTrongGio);
        return vm is null ? NotFound() : View(vm);
    }
 
    public async Task<IActionResult> Create()
        => View(await _service.TaoFormAsync(new Sach { SoLuong = 1 }));
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SachFormViewModel vm)
    {
        ModelState.Remove("Sach.SoLuongCon");
        ModelState.ThemLoiService(await _service.KiemTraAsync(vm.Sach), "Sach.");
        if (!ModelState.IsValid)
        {
            return View(await _service.TaoFormAsync(vm.Sach));
        }
 
        var kq = await _service.TaoAsync(vm.Sach);
        if (!kq.ThanhCong)
        {
            ModelState.ThemLoiService(kq, "Sach.");
            return View(await _service.TaoFormAsync(vm.Sach));
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(QuanLy));
    }
 
    public async Task<IActionResult> Edit(int id)
    {
        var sach = await _service.LayTheoMaAsync(id);
        if (sach is null) return NotFound();
        return View(await _service.TaoFormAsync(sach));
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SachFormViewModel vm)
    {
        if (id != vm.Sach.MaSach) return BadRequest();
        if (await _service.LayTheoMaAsync(id) is null) return NotFound();
 
        ModelState.Remove("Sach.SoLuongCon");
        ModelState.ThemLoiService(await _service.KiemTraAsync(vm.Sach), "Sach.");
        if (!ModelState.IsValid)
        {
            return View(await _service.TaoFormAsync(vm.Sach));
        }
 
        var kq = await _service.CapNhatAsync(id, vm.Sach);
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong)
        {
            ModelState.ThemLoiService(kq, "Sach.");
            return View(await _service.TaoFormAsync(vm.Sach));
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(QuanLy));
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id)
    {
        var kq = await _service.DoiTrangThaiAsync(id);
        if (kq.KhongTimThay) return NotFound();
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(QuanLy));
    }
 
    public async Task<IActionResult> Delete(int id)
    {
        var sach = await _service.LayTheoMaAsync(id);
        if (sach is null) return NotFound();
        ViewBag.CoLichSu = await _service.CoLichSuMuonAsync(id);
        return View(sach);
    }
 
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var kq = await _service.XoaAsync(id);
        if (kq.KhongTimThay) return NotFound();
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(QuanLy));
    }
}
