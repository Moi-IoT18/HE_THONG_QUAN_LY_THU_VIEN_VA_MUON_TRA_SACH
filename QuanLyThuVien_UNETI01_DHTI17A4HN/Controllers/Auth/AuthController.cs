// Họ và tên: Nguyễn Thế Đông
// Mã sinh viên: 23103100187
// Nội dung thực hiện: AuthController xử lý nghiệp vụ đăng nhập, đăng xuất hệ thống (Module 1).

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;

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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
    {
        if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
        {
            ModelState.AddModelError("", "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu");
            return View();
        }

        var user = await _authService.LoginAsync(tenDangNhap, matKhau);
        if (user == null)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác");
            return View();
        }

        // Lưu thông tin vào Session
        HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
        HttpContext.Session.SetString("HoTen", user.HoTen);
        HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
        HttpContext.Session.SetString("VaiTro", user.VaiTro.ToString());

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}