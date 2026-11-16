// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Đăng nhập, đăng xuất và đăng ký an toàn cho bạn đọc.

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.LayMaTaiKhoan().HasValue)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.LayMaTaiKhoan().HasValue)
            return RedirectToAction("Index", "Home");

        return View(new DangKyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(DangKyViewModel model)
    {
        if (HttpContext.Session.LayMaTaiKhoan().HasValue)
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
            return View(model);

        var created = await _authService.RegisterReaderAsync(model);
        if (!created)
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã được sử dụng.");
            return View(model);
        }

        TempData["ThanhCong"] = "Đăng ký thành công. Vui lòng đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var taiKhoan = await _authService.LoginAsync(model.TenDangNhap.Trim(), model.MatKhau);
        if (taiKhoan is null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đang bị khóa.");
            return View(model);
        }

        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32(SessionKeys.MaTaiKhoan, taiKhoan.MaTaiKhoan);
        if (taiKhoan.BanDoc is not null)
            HttpContext.Session.SetInt32(SessionKeys.MaBanDoc, taiKhoan.BanDoc.MaBanDoc);
        HttpContext.Session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
        HttpContext.Session.SetString(SessionKeys.VaiTro, Quyen.TuVaiTro(taiKhoan.VaiTro));

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult TuChoiTruyCap()
    {
        return View();
    }
}
