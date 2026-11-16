// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Dashboard số liệu thư viện dành cho nhân viên và Admin.

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Dashboard;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.Dashboard;

[SessionAuthorize(Roles = Quyen.NhanVienVaQuanLy)]
public sealed class DashboardController : Controller
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet]
    public async Task<IActionResult> Index()
        => View(await _dashboard.LayTongQuanAsync());
}
