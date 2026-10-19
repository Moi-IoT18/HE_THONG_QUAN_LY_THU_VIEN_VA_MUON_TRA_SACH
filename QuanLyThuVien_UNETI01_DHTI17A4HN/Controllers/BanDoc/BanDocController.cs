// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Quản lý bạn đọc (Thủ thư/Admin): danh sách, chi tiết + lịch sử mượn,
//                     thêm (kèm tài khoản), sửa, khóa thẻ, gia hạn thẻ, xóa an toàn.
//                     Hồ sơ cá nhân (Bạn đọc): chỉ xem/sửa hồ sơ của CHÍNH MÌNH, đổi mật khẩu.
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
// Lưu ý: nhiều [Authorize] chồng nhau là điều kiện "VÀ", nên ở đây gắn quyền cho TỪNG action
// (phần quản lý: Nhân viên; phần hồ sơ: Bạn đọc) thay vì gắn ở cấp class.
public class BanDocController : Controller
{
    private readonly IBanDocService _service;
 
    public BanDocController(IBanDocService service)
    {
        _service = service;
    }
 
    // =====================================================================
    // QUẢN LÝ BẠN ĐỌC (Thủ thư, Admin)
    // =====================================================================
 
    [Authorize(Roles = Quyen.NhanVien)]
    public async Task<IActionResult> Index(string? tuKhoa, TrangThaiBanDoc? trangThai, string? tinhTrangThe,
        string sapXep = "ten_asc", int trang = 1)
        => View(await _service.LayDanhSachAsync(tuKhoa, trangThai, tinhTrangThe, sapXep, trang));
 
    [Authorize(Roles = Quyen.NhanVien)]
    public async Task<IActionResult> Details(int id)
    {
        var vm = await _service.LayChiTietAsync(id);
        return vm is null ? NotFound() : View(vm);
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    public IActionResult Create() => View(new BanDocFormViewModel());
 
    [Authorize(Roles = Quyen.NhanVien)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BanDocFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
 
        var kq = await _service.TaoAsync(vm);
        if (!kq.ThanhCong)
        {
            ModelState.ThemLoiService(kq);
            return View(vm);
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Details), new { id = kq.DuLieu });
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _service.LayFormSuaAsync(id);
        return vm is null ? NotFound() : View(vm);
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BanDocFormViewModel vm)
    {
        if (id != vm.MaBanDoc) return BadRequest();
 
        ModelState.Remove(nameof(vm.TenDangNhap)); // không đổi tên đăng nhập ở đây
        if (ModelState.IsValid)
        {
            var kq = await _service.CapNhatAsync(id, vm);
            if (kq.KhongTimThay) return NotFound();
            if (kq.ThanhCong)
            {
                TempData["ThanhCong"] = kq.ThongBao;
                return RedirectToAction(nameof(Details), new { id });
            }
            ModelState.ThemLoiService(kq);
        }
 
        // Hiển thị lại form: lấy tên đăng nhập gốc từ Database
        var goc = await _service.LayFormSuaAsync(id);
        if (goc is null) return NotFound();
        vm.TenDangNhap = goc.TenDangNhap;
        return View(vm);
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoaMo(int id)
    {
        var kq = await _service.KhoaMoAsync(id);
        if (kq.KhongTimThay) return NotFound();
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Details), new { id });
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GiaHanThe(int id)
    {
        var kq = await _service.GiaHanTheAsync(id);
        if (kq.KhongTimThay) return NotFound();
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Details), new { id });
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    public async Task<IActionResult> Delete(int id)
    {
        var bd = await _service.LayKemTaiKhoanAsync(id);
        if (bd is null) return NotFound();
        ViewBag.SoPhieu = await _service.DemPhieuAsync(id);
        return View(bd);
    }
 
    [Authorize(Roles = Quyen.NhanVien)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var kq = await _service.XoaAsync(id);
        if (kq.KhongTimThay) return NotFound();
        if (!kq.ThanhCong)
        {
            TempData["Loi"] = kq.LoiGop;
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ThanhCong"] = kq.ThongBao;
        return RedirectToAction(nameof(Index));
    }
 
    // =====================================================================
    // HỒ SƠ CÁ NHÂN (Bạn đọc) – luôn lấy mã từ Session, không nhận id từ URL
    // =====================================================================
 
    private int? MaTaiKhoan => HttpContext.Session.LayMaTaiKhoan();
 
    [Authorize(Roles = Quyen.BanDoc)]
    public async Task<IActionResult> HoSo()
    {
        var hoSo = await _service.LayHoSoAsync(MaTaiKhoan);
        if (hoSo is null) return RedirectToAction("TuChoiTruyCap", "Auth");
 
        ViewBag.DoiMatKhau = new DoiMatKhauViewModel();
        return View(hoSo);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Quyen.BanDoc)]
    public async Task<IActionResult> HoSo(HoSoViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var kq = await _service.CapNhatHoSoAsync(MaTaiKhoan, vm);
            if (kq.KhongTimThay) return RedirectToAction("TuChoiTruyCap", "Auth");
            if (kq.ThanhCong)
            {
                HttpContext.Session.SetString(SessionKeys.HoTen, kq.DuLieu!);
                TempData["ThanhCong"] = kq.ThongBao;
                return RedirectToAction(nameof(HoSo));
            }
            ModelState.ThemLoiService(kq);
        }
 
        // Hiển thị lại: các trường chỉ đọc lấy từ Database
        var hienThi = await _service.LayHoSoAsync(MaTaiKhoan);
        if (hienThi is null) return RedirectToAction("TuChoiTruyCap", "Auth");
        vm.MaBanDoc = hienThi.MaBanDoc; vm.TenDangNhap = hienThi.TenDangNhap;
        vm.NgayCapThe = hienThi.NgayCapThe; vm.HanThe = hienThi.HanThe;
        vm.TrangThai = hienThi.TrangThai; vm.SoSachDangGiu = hienThi.SoSachDangGiu;
        ViewBag.DoiMatKhau = new DoiMatKhauViewModel();
        return View(vm);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Quyen.BanDoc)]
    public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Loi"] = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(HoSo));
        }
 
        var kq = await _service.DoiMatKhauAsync(MaTaiKhoan, vm);
        if (kq.KhongTimThay) return RedirectToAction("TuChoiTruyCap", "Auth");
        TempData[kq.ThanhCong ? "ThanhCong" : "Loi"] = kq.ThanhCong ? kq.ThongBao : kq.LoiGop;
        return RedirectToAction(nameof(HoSo));
    }
}
