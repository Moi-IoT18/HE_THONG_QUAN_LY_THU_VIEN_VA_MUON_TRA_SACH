// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Hợp đồng lấy các chỉ số tổng quan Dashboard.

using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardViewModel> LayTongQuanAsync();
}
