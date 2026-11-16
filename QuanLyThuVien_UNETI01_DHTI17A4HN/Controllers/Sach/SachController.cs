// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: CRUD sách, tìm kiếm, lọc, sắp xếp và phân trang (Module 2).

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TheLoai;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
using SachEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Sach;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.Sach;

public class SachController : Controller
{
    private readonly ISachService _sachService;
    private readonly IBookImageStorage _bookImageStorage;
    private readonly ITheLoaiService _theLoaiService;
    private readonly INhaXuatBanService _nhaXuatBanService;

    public SachController(ISachService sachService, IBookImageStorage bookImageStorage, ITheLoaiService theLoaiService,
        INhaXuatBanService nhaXuatBanService)
    {
        _sachService = sachService;
        _bookImageStorage = bookImageStorage;
        _theLoaiService = theLoaiService;
        _nhaXuatBanService = nhaXuatBanService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SachFilterViewModel filter, int trang = 1)
    {
        if (filter.GiaTu.HasValue && filter.GiaDen.HasValue && filter.GiaTu > filter.GiaDen)
            ModelState.AddModelError(nameof(filter.GiaTu), "Giá từ không được lớn hơn giá đến.");

        if (!ModelState.IsValid)
        {
            filter.GiaTu = null;
            filter.GiaDen = null;
        }

        ViewBag.TheLoais = new SelectList(await _theLoaiService.GetAllAsync(), "MaTheLoai", "TenTheLoai", filter.MaTheLoai);
        var result = await _sachService.LayDanhSachSachAsync(filter, trang);
        return Request.Headers.XRequestedWith == "XMLHttpRequest"
            ? PartialView("_DanhSach", result)
            : View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var sach = await _sachService.LaySachTheoIdAsync(id);
        return sach is null ? NotFound() : View(sach);
    }

    [HttpGet]
    [SessionAuthorize(Roles = Quyen.NhanVien + "," + Quyen.QuanLy)]
    public async Task<IActionResult> Create()
    {
        await NapDanhSachKhoaNgoaiAsync();
        return View(new SachViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [SessionAuthorize(Roles = Quyen.NhanVien + "," + Quyen.QuanLy)]
    public async Task<IActionResult> Create(SachViewModel model)
    {
        await KiemTraFormAsync(model);
        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoaNgoaiAsync();
            return View(model);
        }

        string? imagePath;
        try
        {
            imagePath = await _bookImageStorage.SaveAsync(model.AnhBia);
        }
        catch (InvalidDataException ex)
        {
            ModelState.AddModelError(nameof(model.AnhBia), ex.Message);
            await NapDanhSachKhoaNgoaiAsync();
            return View(model);
        }

        var sach = new SachEntity();
        GanThongTin(sach, model);
        sach.DuongDanAnhBia = imagePath;
        var saved = false;
        try
        {
            await _sachService.ThemSachAsync(sach);
            saved = true;
        }
        finally
        {
            if (!saved && imagePath is not null)
                await _bookImageStorage.DeleteAsync(imagePath);
        }
        TempData["ThanhCong"] = "Thêm sách thành công.";
        return RedirectToAction(nameof(Details), new { id = sach.MaSach });
    }

    [HttpGet]
    [SessionAuthorize(Roles = Quyen.NhanVien + "," + Quyen.QuanLy)]
    public async Task<IActionResult> Edit(int id)
    {
        var sach = await _sachService.LaySachTheoIdAsync(id);
        if (sach is null) return NotFound();

        await NapDanhSachKhoaNgoaiAsync(sach.MaTheLoai, sach.MaNhaXuatBan);
        return View(TaoForm(sach));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [SessionAuthorize(Roles = Quyen.NhanVien + "," + Quyen.QuanLy)]
    public async Task<IActionResult> Edit(int id, SachViewModel model)
    {
        if (id != model.MaSach) return BadRequest();
        var sach = await _sachService.LaySachTheoIdAsync(id);
        if (sach is null) return NotFound();

        model.DuongDanAnhBiaHienTai = sach.DuongDanAnhBia;
        await KiemTraFormAsync(model, choPhepNgungHoatDong: true);
        var soLuongDangMuon = sach.SoLuong - sach.SoLuongCon;
        if (model.SoLuong < soLuongDangMuon)
            ModelState.AddModelError(nameof(model.SoLuong),
                $"Tổng số lượng không được nhỏ hơn {soLuongDangMuon} cuốn đang được mượn.");
        if (model.SoLuongCon > model.SoLuong - soLuongDangMuon)
            ModelState.AddModelError(nameof(model.SoLuongCon),
                "Số lượng còn vượt quá số lượng có thể lưu kho sau khi tính sách đang được mượn.");

        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoaNgoaiAsync(sach.MaTheLoai, sach.MaNhaXuatBan);
            model.DuongDanAnhBiaHienTai = sach.DuongDanAnhBia;
            return View(model);
        }

        string? uploadedImage = null;
        try
        {
            uploadedImage = await _bookImageStorage.SaveAsync(model.AnhBia);
        }
        catch (InvalidDataException ex)
        {
            ModelState.AddModelError(nameof(model.AnhBia), ex.Message);
            await NapDanhSachKhoaNgoaiAsync(sach.MaTheLoai, sach.MaNhaXuatBan);
            model.DuongDanAnhBiaHienTai = sach.DuongDanAnhBia;
            return View(model);
        }

        var oldImage = sach.DuongDanAnhBia;
        GanThongTin(sach, model);
        if (uploadedImage is not null)
            sach.DuongDanAnhBia = uploadedImage;
        else if (model.XoaAnhBia)
            sach.DuongDanAnhBia = null;

        var saved = false;
        try
        {
            await _sachService.CapNhatSachAsync(sach);
            saved = true;
        }
        finally
        {
            if (!saved && uploadedImage is not null)
                await _bookImageStorage.DeleteAsync(uploadedImage);
        }
        if (saved && oldImage is not null && oldImage != sach.DuongDanAnhBia)
            await _bookImageStorage.DeleteAsync(oldImage);
        TempData["ThanhCong"] = "Cập nhật sách thành công.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [SessionAuthorize(Roles = Quyen.NhanVien + "," + Quyen.QuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var sach = await _sachService.LaySachTheoIdAsync(id);
        if (sach is null) return NotFound();
        return View(sach);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [SessionAuthorize(Roles = Quyen.NhanVien + "," + Quyen.QuanLy)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sach = await _sachService.LaySachTheoIdAsync(id);
        if (sach is null) return NotFound();

        if (!await _sachService.XoaSachAsync(id))
        {
            TempData["Loi"] = "Không thể xóa sách đã có lịch sử mượn; hãy chuyển trạng thái sang ngừng phục vụ.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await _bookImageStorage.DeleteAsync(sach.DuongDanAnhBia);
        TempData["ThanhCong"] = "Đã xóa sách.";
        return RedirectToAction(nameof(Index));
    }

    private async Task KiemTraFormAsync(SachViewModel model, bool choPhepNgungHoatDong = false)
    {
        if (model.NamXuatBan > DateTime.Today.Year)
            ModelState.AddModelError(nameof(model.NamXuatBan), "Năm xuất bản không được lớn hơn năm hiện tại.");
        if (model.SoLuongCon > model.SoLuong)
            ModelState.AddModelError(nameof(model.SoLuongCon), "Số lượng còn không được lớn hơn tổng số lượng.");
        if (!Enum.IsDefined(model.TrangThai))
            ModelState.AddModelError(nameof(model.TrangThai), "Trạng thái sách không hợp lệ.");
        if (!await _sachService.KhoaNgoaiHopLeAsync(model.MaTheLoai, model.MaNhaXuatBan, choPhepNgungHoatDong))
            ModelState.AddModelError(string.Empty, "Vui lòng chọn thể loại và nhà xuất bản đang hoạt động.");
    }

    private async Task NapDanhSachKhoaNgoaiAsync(int? maTheLoaiHienTai = null, int? maNhaXuatBanHienTai = null)
    {
        var theLoais = await _theLoaiService.GetAllAsync();
        var nhaXuatBans = await _nhaXuatBanService.GetAllAsync();
        if (maTheLoaiHienTai.HasValue)
            theLoais = theLoais.Where(t => t.TrangThai || t.MaTheLoai == maTheLoaiHienTai).ToList();
        else
            theLoais = theLoais.Where(t => t.TrangThai).ToList();
        if (maNhaXuatBanHienTai.HasValue)
            nhaXuatBans = nhaXuatBans.Where(n => n.TrangThai || n.MaNhaXuatBan == maNhaXuatBanHienTai).ToList();
        else
            nhaXuatBans = nhaXuatBans.Where(n => n.TrangThai).ToList();

        ViewBag.TheLoais = new SelectList(theLoais, "MaTheLoai", "TenTheLoai", maTheLoaiHienTai);
        ViewBag.NhaXuatBans = new SelectList(nhaXuatBans,
            "MaNhaXuatBan", "TenNhaXuatBan", maNhaXuatBanHienTai);
    }

    private static SachViewModel TaoForm(SachEntity sach) => new()
    {
        MaSach = sach.MaSach,
        TenSach = sach.TenSach,
        TacGia = sach.TacGia,
        MaTheLoai = sach.MaTheLoai,
        MaNhaXuatBan = sach.MaNhaXuatBan,
        NamXuatBan = sach.NamXuatBan,
        SoLuong = sach.SoLuong,
        SoLuongCon = sach.SoLuongCon,
        DonGia = sach.DonGia,
        ViTriKeSach = sach.ViTriKeSach,
        TrangThai = sach.TrangThai,
        GhiChu = sach.GhiChu,
        DuongDanAnhBiaHienTai = sach.DuongDanAnhBia
    };

    private static void GanThongTin(SachEntity sach, SachViewModel model)
    {
        sach.TenSach = model.TenSach.Trim();
        sach.TacGia = model.TacGia?.Trim();
        sach.MaTheLoai = model.MaTheLoai;
        sach.MaNhaXuatBan = model.MaNhaXuatBan;
        sach.NamXuatBan = model.NamXuatBan;
        sach.SoLuong = model.SoLuong;
        sach.SoLuongCon = model.SoLuongCon;
        sach.DonGia = model.DonGia;
        sach.ViTriKeSach = model.ViTriKeSach?.Trim();
        sach.TrangThai = model.TrangThai;
        sach.GhiChu = model.GhiChu?.Trim();
    }
}
