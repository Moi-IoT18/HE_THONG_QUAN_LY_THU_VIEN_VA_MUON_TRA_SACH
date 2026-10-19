// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Quản lý chi tiết mượn trong ngữ cảnh 1 phiếu (Thủ thư/Admin).
//   Chi tiết gắn với phiếu, không tồn tại độc lập. Chỉ được thêm/sửa/xóa khi phiếu đang "Chờ duyệt";
//   sau khi đã duyệt/trả, kết quả do hệ thống xác nhận và không được sửa tay.
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.NhanVien)]
public class ChiTietMuonController : Controller
{
    private readonly IChiTietMuonService _service;
 
    public ChiTietMuonController(IChiTietMuonService service)
    {
        _service = service;
    }
 
    /// <summary>Danh sách chi tiết của 1 phiếu + form thêm sách.</summary>
    public async Task<IActionResult> Index(int maPhieu)
    {
        var vm = await _service.LayDanhSachAsync(maPhieu);
        return vm is null ? NotFound() : View(vm);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int maPhieu, int maSach, int soLuong = 1)
    {
        var kq = await _service.ThemAsync(maPhieu, maSach, soLuong);
        if (kq.KhongTimThay) return NotFound();
        TempData[kq.ThanhCong ? "ThanhCong" : "Loi"] = kq.ThanhCong ? kq.ThongBao : kq.LoiGop;
        return RedirectToAction(nameof(Index), new { maPhieu });
    }
 
    public async Task<IActionResult> Edit(int id)
    {
        var kq = await _service.LayFormSuaAsync(id);
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong)
        {
            TempData["Loi"] = kq.LoiGop;
            return RedirectToAction(nameof(Index), new { maPhieu = kq.DuLieu!.MaPhieuMuon });
        }
        return View(kq.DuLieu);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ChiTietMuonFormViewModel vm)
    {
        if (id != vm.MaChiTiet) return BadRequest();
 
        // Kiểm tra phiếu còn "Chờ duyệt" và lấy dữ liệu gốc (mã phiếu, tên sách)
        var goc = await _service.LayFormSuaAsync(id);
        if (goc.KhongTimThay) return NotFound();
        var maPhieu = goc.DuLieu!.MaPhieuMuon;
        if (!goc.ThanhCong)
        {
            TempData["Loi"] = goc.LoiGop;
            return RedirectToAction(nameof(Index), new { maPhieu });
        }
 
        ModelState.Remove(nameof(vm.TenSach));
        if (ModelState.IsValid)
        {
            var kq = await _service.CapNhatAsync(id, vm);
            if (kq.ThanhCong)
            {
                TempData["ThanhCong"] = kq.ThongBao;
                return RedirectToAction(nameof(Index), new { maPhieu });
            }
            ModelState.ThemLoiService(kq);
        }
 
        vm.TenSach = goc.DuLieu.TenSach;
        vm.SoLuongCon = goc.DuLieu.SoLuongCon;
        return View(vm);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var kq = await _service.XoaAsync(id);
        if (kq.KhongTimThay) return NotFound();
        TempData[kq.ThanhCong ? "ThanhCong" : "Loi"] = kq.ThanhCong ? kq.ThongBao : kq.LoiGop;
        return RedirectToAction(nameof(Index), new { maPhieu = kq.DuLieu });
    }
}
