// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Hiển thị thống kê nghiệp vụ được tính bằng LINQ/EF Core.

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.ThongKe;

[SessionAuthorize(Roles = Quyen.NhanVienVaQuanLy)]
public sealed class ThongKeController : Controller
{
    private readonly IThongKeService _thongKe;
    private readonly IThongKeExportService _export;

    public ThongKeController(IThongKeService thongKe, IThongKeExportService export)
    {
        _thongKe = thongKe;
        _export = export;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
        => View(await _thongKe.LayThongKeAsync());

    [HttpGet]
    public async Task<IActionResult> Excel()
    {
        var report = await _thongKe.LayThongKeAsync();
        return File(_export.TaoExcel(report),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"ThongKeThuVien-{DateTime.Today:yyyyMMdd}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> Pdf()
    {
        var report = await _thongKe.LayThongKeAsync();
        return File(_export.TaoPdf(report), "application/pdf",
            $"ThongKeThuVien-{DateTime.Today:yyyyMMdd}.pdf");
    }
}
