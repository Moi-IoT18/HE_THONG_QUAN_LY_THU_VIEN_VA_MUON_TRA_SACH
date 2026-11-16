using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
using TaiKhoanEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.TaiKhoan;

public class TaiKhoanController : Controller
{
    private readonly ITaiKhoanService _service;

    public TaiKhoanController(ITaiKhoanService service)
    {
        _service = service;
    }

    private bool IsAdmin => HttpContext.Session.LayVaiTro() == Quyen.QuanLy;

    private IActionResult? RequireLogin()
        => HttpContext.Session.DaDangNhap() ? null : RedirectToAction("Login", "Auth");

    private IActionResult? RequireAdmin()
        => IsAdmin ? null : RedirectToAction("TuChoiTruyCap", "Auth");

    public async Task<IActionResult> Index()
    {
        if (RequireLogin() is { } loginRedirect) return loginRedirect;
        if (RequireAdmin() is { } adminRedirect) return adminRedirect;

        return View(await _service.GetAllAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (RequireLogin() is { } loginRedirect) return loginRedirect;
        if (RequireAdmin() is { } adminRedirect) return adminRedirect;

        return View(new TaiKhoanCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaiKhoanCreateViewModel model)
    {
        if (RequireLogin() is { } loginRedirect) return loginRedirect;
        if (RequireAdmin() is { } adminRedirect) return adminRedirect;

        model.TenDangNhap = model.TenDangNhap?.Trim() ?? string.Empty;
        if (model.TenDangNhap.Length > 0 && await _service.IsDuplicateAsync(model.TenDangNhap))
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");

        if (!Enum.IsDefined(model.VaiTro))
            ModelState.AddModelError(nameof(model.VaiTro), "Vai trò không hợp lệ.");

        if (!ModelState.IsValid) return View(model);

        var taiKhoan = new TaiKhoanEntity
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = MatKhauHelper.BamMatKhau(model.MatKhau),
            HoTen = model.HoTen.Trim(),
            Email = model.Email?.Trim(),
            VaiTro = model.VaiTro,
            TrangThai = model.TrangThai,
            NgayTao = DateTime.Now
        };
        if (taiKhoan.VaiTro == VaiTro.BanDoc)
        {
            taiKhoan.BanDoc = new BanDoc
            {
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                NgayCapThe = DateTime.Today,
                HanThe = DateTime.Today.AddYears(1),
                TrangThai = TrangThaiBanDoc.HoatDong
            };
        }

        await _service.AddAsync(taiKhoan);
        TempData["ThanhCong"] = "Tạo tài khoản thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (RequireLogin() is { } loginRedirect) return loginRedirect;
        if (!IsAdmin && HttpContext.Session.LayMaTaiKhoan() != id)
            return RedirectToAction("TuChoiTruyCap", "Auth");

        var taiKhoan = await _service.GetByIdAsync(id);
        return taiKhoan is null ? NotFound() : View(taiKhoan);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (RequireLogin() is { } loginRedirect) return loginRedirect;
        if (!IsAdmin && HttpContext.Session.LayMaTaiKhoan() != id)
            return RedirectToAction("TuChoiTruyCap", "Auth");

        var taiKhoan = await _service.GetByIdAsync(id);
        if (taiKhoan is null) return NotFound();

        ViewBag.IsAdmin = IsAdmin;
        return View(new TaiKhoanEditViewModel
        {
            MaTaiKhoan = taiKhoan.MaTaiKhoan,
            HoTen = taiKhoan.HoTen,
            Email = taiKhoan.Email,
            VaiTro = taiKhoan.VaiTro,
            TrangThai = taiKhoan.TrangThai
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaiKhoanEditViewModel model)
    {
        if (RequireLogin() is { } loginRedirect) return loginRedirect;
        if (!IsAdmin && HttpContext.Session.LayMaTaiKhoan() != id)
            return RedirectToAction("TuChoiTruyCap", "Auth");
        if (id != model.MaTaiKhoan) return BadRequest();

        var taiKhoan = await _service.GetByIdAsync(id);
        if (taiKhoan is null) return NotFound();

        if (IsAdmin && !Enum.IsDefined(model.VaiTro))
            ModelState.AddModelError(nameof(model.VaiTro), "Vai trò không hợp lệ.");

        var currentUserId = HttpContext.Session.LayMaTaiKhoan();
        var isEditingSelf = currentUserId == id;

        if (IsAdmin && isEditingSelf)
        {
            if (!model.TrangThai)
                ModelState.AddModelError(nameof(model.TrangThai), "Bạn không thể tự khóa tài khoản quản trị của chính mình.");
            if (model.VaiTro != VaiTro.QuanLy)
                ModelState.AddModelError(nameof(model.VaiTro), "Bạn không thể tự giáng cấp vai trò quản trị của chính mình.");
        }

        if (!string.IsNullOrWhiteSpace(model.MatKhauMoi) && (!IsAdmin || isEditingSelf))
        {
            if (string.IsNullOrWhiteSpace(model.MatKhauCu))
                ModelState.AddModelError(nameof(model.MatKhauCu), "Vui lòng nhập mật khẩu hiện tại để đổi mật khẩu.");
            else if (!MatKhauHelper.KiemTra(model.MatKhauCu, taiKhoan.MatKhau))
                ModelState.AddModelError(nameof(model.MatKhauCu), "Mật khẩu hiện tại không chính xác.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.IsAdmin = IsAdmin;
            return View(model);
        }

        taiKhoan.HoTen = model.HoTen.Trim();
        taiKhoan.Email = model.Email?.Trim();
        if (!string.IsNullOrWhiteSpace(model.MatKhauMoi))
            taiKhoan.MatKhau = MatKhauHelper.BamMatKhau(model.MatKhauMoi);
        if (IsAdmin)
        {
            taiKhoan.VaiTro = model.VaiTro;
            taiKhoan.TrangThai = model.TrangThai;
            if (model.VaiTro == VaiTro.BanDoc && taiKhoan.BanDoc is null)
            {
                taiKhoan.BanDoc = new BanDoc
                {
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    NgayCapThe = DateTime.Today,
                    HanThe = DateTime.Today.AddYears(1),
                    TrangThai = TrangThaiBanDoc.HoatDong
                };
            }
        }

        if (taiKhoan.BanDoc is not null)
        {
            taiKhoan.BanDoc.HoTen = taiKhoan.HoTen;
            taiKhoan.BanDoc.Email = taiKhoan.Email ?? string.Empty;
        }

        await _service.UpdateAsync(taiKhoan);
        if (isEditingSelf)
            HttpContext.Session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
        TempData["ThanhCong"] = "Cập nhật tài khoản thành công.";
        return RedirectToAction(IsAdmin ? nameof(Index) : nameof(Details), IsAdmin ? null : new { id });
    }
}
