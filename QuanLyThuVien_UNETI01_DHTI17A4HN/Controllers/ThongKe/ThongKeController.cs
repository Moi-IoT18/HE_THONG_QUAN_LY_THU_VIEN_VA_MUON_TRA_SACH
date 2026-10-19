// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Trang thống kê (Thủ thư/Admin). Truy vấn LINQ nằm ở IThongKeService.
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers;
 
[Authorize(Roles = Quyen.NhanVien)]
public class ThongKeController : Controller
{
    private readonly IThongKeService _service;
 
    public ThongKeController(IThongKeService service)
    {
        _service = service;
    }
 
    public async Task<IActionResult> Index(int? nam) => View(await _service.LayThongKeAsync(nam));
}
