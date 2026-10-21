// Họ và tên: Nguyễn Thế Đông
// Mã sinh viên: 23103100187
// Nội dung thực hiện: TaiKhoanController quản lý danh sách và cập nhật thông tin tài khoản người dùng (Module 1).

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;

public class TaiKhoanController : Controller
{
    private readonly AppDbContext _context;

    public TaiKhoanController(AppDbContext context)
    {
        _context = context;
    }

    // GET: TaiKhoan
    public async Task<IActionResult> Index()
    {
        var list = await _context.TaiKhoans.ToListAsync();
        return View(list);
    }

    // GET: TaiKhoan/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var taiKhoan = await _context.TaiKhoans.FindAsync(id);
        if (taiKhoan == null) return NotFound();
        return View(taiKhoan);
    }

    // POST: TaiKhoan/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaiKhoan taiKhoan)
    {
        if (id != taiKhoan.MaTaiKhoan) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var userInDb = await _context.TaiKhoans.FindAsync(id);
                if (userInDb != null)
                {
                    userInDb.HoTen = taiKhoan.HoTen;
                    userInDb.Email = taiKhoan.Email;
                    userInDb.VaiTro = taiKhoan.VaiTro;
                    userInDb.TrangThai = taiKhoan.TrangThai;
                    if (!string.IsNullOrEmpty(taiKhoan.MatKhau))
                    {
                        userInDb.MatKhau = taiKhoan.MatKhau;
                    }
                    _context.Update(userInDb);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(_context.TaiKhoans.Any(e => e.MaTaiKhoan == id))) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(taiKhoan);
    }
}