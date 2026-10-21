// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Xây dựng Controller quản lý Sách (Module 2).

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.Sach;

public class SachController : Controller
{
    private readonly SachService _sachService;

    public SachController(SachService sachService)
    {
        _sachService = sachService;
    }

    // GET: /Sach
    public async Task<IActionResult> Index()
    {
        var danhSachSach = await _sachService.LayDanhSachSachAsync();
        return View(danhSachSach);
    }

    // GET: /Sach/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var sach = await _sachService.LaySachTheoIdAsync(id);
        if (sach == null)
        {
            return NotFound();
        }
        return View(sach);
    }
}