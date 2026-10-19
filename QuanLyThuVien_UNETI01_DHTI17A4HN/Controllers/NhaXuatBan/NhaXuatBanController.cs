// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Module 2 – Quản lý nhà xuất bản (Thủ thư/Admin): danh sách, chi tiết, thêm,
//                     sửa, xóa an toàn. Nghiệp vụ nằm ở INhaXuatBanService.
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.NhanVien)]
public class NhaXuatBanController : Controller
{
    private readonly INhaXuatBanService _service;
 
    public NhaXuatBanController(INhaXuatBanService service)
    {
        _service = service;
    }
 
    public async Task<IActionResult> Index(string? tuKhoa, bool? trangThai, int trang = 1)
        => View(await _service.LayDanhSachAsync(tuKhoa, trangThai, trang));
 
    public async Task<IActionResult> Details(int id)
    {
        var nxb = await _service.LayChiTietAsync(id);
        return nxb is null ? NotFound() : View(nxb);
    }
 
    public IActionResult Create() => View(new NhaXuatBan { TrangThai = true });
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TenNhaXuatBan,DiaChi,SoDienThoai,Email,TrangThai")] NhaXuatBan nxb)
    {
        if (!ModelState.IsValid) return View(nxb);
 
        var kq = await _service.TaoAsync(nxb);
        if (!kq.ThanhCong)
        {
            ModelState.ThemLoiService(kq);
            return View(nxb);
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Index));
    }
 
    public async Task<IActionResult> Edit(int id)
    {
        var nxb = await _service.LayTheoMaAsync(id);
        return nxb is null ? NotFound() : View(nxb);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("MaNhaXuatBan,TenNhaXuatBan,DiaChi,SoDienThoai,Email,TrangThai")] NhaXuatBan nxb)
    {
        if (id != nxb.MaNhaXuatBan) return BadRequest();
        if (!ModelState.IsValid) return View(nxb);
 
        var kq = await _service.CapNhatAsync(id, nxb);
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong)
        {
            ModelState.ThemLoiService(kq);
            return View(nxb);
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Index));
    }
 
    public async Task<IActionResult> Delete(int id)
    {
        var nxb = await _service.LayTheoMaAsync(id);
        if (nxb is null) NotFound();
        ViewBag.SoSach = await _service.DemSachAsync(id);
        return View(nxb);
    }
 
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var kq = await _service.XoaAsync(id);
        if (kq.KhongTimThay) return NotFound();
        TempData[kq.ThanhCong ? "ThanhCong" : "Loi"] = kq.ThanhCong ? kq.ThongBao : kq.LoiGop;
        return RedirectToAction(nameof(Index));
    }
}
