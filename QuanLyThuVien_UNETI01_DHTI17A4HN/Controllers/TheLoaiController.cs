using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TheLoai;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;

public class TheLoaiController : Controller
{
    private readonly ITheLoaiService _service;

    public TheLoaiController(ITheLoaiService service)
    {
        _service = service;
    }

    private bool IsAdmin => HttpContext.Session.LayVaiTro() == Quyen.QuanLy;

    public async Task<IActionResult> Index()
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        var list = await _service.GetAllAsync();
        return View(list);
    }

    public IActionResult Create()
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");
        return View(new TheLoai());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TheLoai model)
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        if (string.IsNullOrWhiteSpace(model.TenTheLoai))
        {
            ModelState.AddModelError(nameof(model.TenTheLoai), "Tên thể loại bắt buộc.");
        }
        else if (await _service.IsDuplicateAsync(model.TenTheLoai))
        {
            ModelState.AddModelError(nameof(model.TenTheLoai), "Tên thể loại đã tồn tại.");
        }

        if (!ModelState.IsValid) return View(model);

        model.TenTheLoai = model.TenTheLoai.Trim();
        if (model.ThuTuHienThi <= 0) model.ThuTuHienThi = 1;

        await _service.AddAsync(new TheLoai
        {
            TenTheLoai = model.TenTheLoai,
            MoTa = model.MoTa,
            TrangThai = model.TrangThai,
            ThuTuHienThi = model.ThuTuHienThi
        });
        TempData["ThanhCong"] = "Thêm thể loại thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TheLoai model)
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        if (id != model.MaTheLoai) return BadRequest();
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();

        if (string.IsNullOrWhiteSpace(model.TenTheLoai))
        {
            ModelState.AddModelError(nameof(model.TenTheLoai), "Tên thể loại bắt buộc.");
        }
        else if (await _service.IsDuplicateAsync(model.TenTheLoai, id))
        {
            ModelState.AddModelError(nameof(model.TenTheLoai), "Tên thể loại đã tồn tại.");
        }

        if (!ModelState.IsValid) return View(model);

        model.TenTheLoai = model.TenTheLoai.Trim();
        if (model.ThuTuHienThi <= 0) model.ThuTuHienThi = 1;

        item.TenTheLoai = model.TenTheLoai;
        item.MoTa = model.MoTa;
        item.TrangThai = model.TrangThai;
        item.ThuTuHienThi = model.ThuTuHienThi;
        await _service.UpdateAsync(item);
        TempData["ThanhCong"] = "Cập nhật thể loại thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        ViewBag.InUse = await _service.IsInUseAsync(id);
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!HttpContext.Session.DaDangNhap()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return RedirectToAction("TuChoiTruyCap", "Auth");

        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        if (await _service.IsInUseAsync(id))
        {
            TempData["Loi"] = "Không thể xóa thể loại vì dữ liệu này đã được tham chiếu bởi sách.";
            return RedirectToAction(nameof(Index));
        }

        await _service.DeleteAsync(item);
        TempData["ThanhCong"] = "Xóa thể loại thành công.";
        return RedirectToAction(nameof(Index));
    }
}
