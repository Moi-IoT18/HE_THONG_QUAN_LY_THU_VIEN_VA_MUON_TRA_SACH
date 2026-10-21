// Họ và tên: Nguyễn Thế Đông
// Mã sinh viên: 23103100187
// Nội dung thực hiện: TheLoaiController quản lý các chức năng CRUD thể loại sách (Module 1).

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;

public class TheLoaiController : Controller
{
    private readonly AppDbContext _context;

    public TheLoaiController(AppDbContext context)
    {
        _context = context;
    }

    // GET: TheLoai
    public async Task<IActionResult> Index()
    {
        var list = await _context.TheLoais.ToListAsync();
        return View(list);
    }

    // GET: TheLoai/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TheLoai/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TheLoai theLoai)
    {
        if (ModelState.IsValid)
        {
            _context.Add(theLoai);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(theLoai);
    }

    // GET: TheLoai/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var theLoai = await _context.TheLoais.FindAsync(id);
        if (theLoai == null) return NotFound();
        return View(theLoai);
    }

    // POST: TheLoai/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TheLoai theLoai)
    {
        if (id != theLoai.MaTheLoai) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(theLoai);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(_context.TheLoais.Any(e => e.MaTheLoai == id))) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(theLoai);
    }

    // GET: TheLoai/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var theLoai = await _context.TheLoais
            .FirstOrDefaultAsync(m => m.MaTheLoai == id);
        if (theLoai == null) return NotFound();

        return View(theLoai);
    }

    // POST: TheLoai/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var theLoai = await _context.TheLoais.FindAsync(id);
        if (theLoai != null)
        {
            _context.TheLoais.Remove(theLoai);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}