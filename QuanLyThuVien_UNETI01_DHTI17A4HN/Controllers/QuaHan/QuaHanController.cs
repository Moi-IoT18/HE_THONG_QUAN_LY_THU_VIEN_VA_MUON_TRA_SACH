// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Hiển thị các phiếu quá hạn cho nhân viên và Admin.

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.QuaHan;

[SessionAuthorize(Roles = Quyen.NhanVienVaQuanLy)]
public sealed class QuaHanController : Controller
{
    private readonly IQuaHanService _quaHan;

    public QuaHanController(IQuaHanService quaHan) => _quaHan = quaHan;

    [HttpGet]
    public async Task<IActionResult> Index()
        => View(await _quaHan.LayDanhSachQuaHanAsync());
}
