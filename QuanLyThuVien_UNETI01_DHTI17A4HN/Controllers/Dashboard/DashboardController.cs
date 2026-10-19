// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Dashboard tổng quan (Thủ thư/Admin).
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.NhanVien)]
public class DashboardController : Controller
{
    private readonly IDashboardService _service;
 
    public DashboardController(IDashboardService service)
    {
        _service = service;
    }
 
    public async Task<IActionResult> Index() => View(await _service.LayDashboardAsync());
}
