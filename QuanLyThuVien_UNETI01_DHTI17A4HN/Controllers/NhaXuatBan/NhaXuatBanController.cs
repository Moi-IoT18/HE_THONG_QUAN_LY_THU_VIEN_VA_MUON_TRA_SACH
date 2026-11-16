// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: CRUD nhà xuất bản, kiểm tra quyền và bảo vệ khóa ngoại sách (Module 2).

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
using NhaXuatBanEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.NhaXuatBan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.NhaXuatBan;

[SessionAuthorize(Roles = Quyen.QuanLy)]
public class NhaXuatBanController : Controller
{
    private readonly INhaXuatBanService _service;

    public NhaXuatBanController(INhaXuatBanService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    [HttpGet]
    public IActionResult Create()
        => View(new NhaXuatBanViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhaXuatBanViewModel model)
    {
        model.TenNhaXuatBan = model.TenNhaXuatBan?.Trim() ?? string.Empty;
        if (model.TenNhaXuatBan.Length > 0 && await _service.IsDuplicateAsync(model.TenNhaXuatBan))
            ModelState.AddModelError(nameof(model.TenNhaXuatBan), "Tên nhà xuất bản đã tồn tại.");

        if (!ModelState.IsValid) return View(model);

        await _service.AddAsync(new NhaXuatBanEntity
        {
            TenNhaXuatBan = model.TenNhaXuatBan,
            DiaChi = model.DiaChi?.Trim(),
            SoDienThoai = model.SoDienThoai?.Trim(),
            Email = model.Email?.Trim(),
            TrangThai = model.TrangThai
        });
        TempData["ThanhCong"] = "Thêm nhà xuất bản thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return View(new NhaXuatBanViewModel
        {
            MaNhaXuatBan = entity.MaNhaXuatBan,
            TenNhaXuatBan = entity.TenNhaXuatBan,
            DiaChi = entity.DiaChi,
            SoDienThoai = entity.SoDienThoai,
            Email = entity.Email,
            TrangThai = entity.TrangThai
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NhaXuatBanViewModel model)
    {
        if (id != model.MaNhaXuatBan) return BadRequest();
        var entity = await _service.GetByIdAsync(id);
        if (entity is null) return NotFound();

        model.TenNhaXuatBan = model.TenNhaXuatBan?.Trim() ?? string.Empty;
        if (model.TenNhaXuatBan.Length > 0
            && await _service.IsDuplicateAsync(model.TenNhaXuatBan, id))
            ModelState.AddModelError(nameof(model.TenNhaXuatBan), "Tên nhà xuất bản đã tồn tại.");
        if (!ModelState.IsValid) return View(model);

        entity.TenNhaXuatBan = model.TenNhaXuatBan;
        entity.DiaChi = model.DiaChi?.Trim();
        entity.SoDienThoai = model.SoDienThoai?.Trim();
        entity.Email = model.Email?.Trim();
        entity.TrangThai = model.TrangThai;
        await _service.UpdateAsync(entity);
        TempData["ThanhCong"] = "Cập nhật nhà xuất bản thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity is null) return NotFound();
        ViewBag.InUse = await _service.IsInUseAsync(id);
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity is null) return NotFound();
        if (!await _service.DeleteAsync(id))
        {
            TempData["Loi"] = "Không thể xóa nhà xuất bản đang được sách tham chiếu.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["ThanhCong"] = "Đã xóa nhà xuất bản.";
        return RedirectToAction(nameof(Index));
    }
}
