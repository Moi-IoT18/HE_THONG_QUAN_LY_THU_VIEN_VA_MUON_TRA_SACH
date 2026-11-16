// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Hợp đồng thống kê LINQ Module 5.

using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;

public interface IThongKeService
{
    Task<ThongKeViewModel> LayThongKeAsync();
}
