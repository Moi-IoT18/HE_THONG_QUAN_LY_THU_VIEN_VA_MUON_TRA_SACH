// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Xử lý quá hạn (Thủ thư/Admin): quét phiếu quá hạn, danh sách phiếu quá hạn
//                     kèm số ngày trễ và tiền phạt dự kiến, tìm theo bạn đọc, phân trang.
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.NhanVien)]
public class QuaHanController : Controller
{
    private readonly IQuaHanService _service;
 
    public QuaHanController(IQuaHanService service)
    {
        _service = service;
    }
 
    public async Task<IActionResult> Index(string? tuKhoa, int trang = 1)
        => View(await _service.LayDanhSachAsync(tuKhoa, trang));
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quet()
    {
        var soPhieu = await _service.QuetQuaHanAsync();
        TempData["ThanhCong"] = soPhieu > 0
            ? $"Đã chuyển {soPhieu} phiếu sang trạng thái Quá hạn."
            : "Không có phiếu nào mới quá hạn.";
        return RedirectToAction(nameof(Index));
    }
}
