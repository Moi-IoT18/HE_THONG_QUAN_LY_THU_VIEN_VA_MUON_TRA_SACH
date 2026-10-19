// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Interface thống kê.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public interface IThongKeService
{
    /// <summary>Thống kê theo năm (null = năm gần nhất có phiếu mượn).</summary>
    Task<ThongKeViewModel> LayThongKeAsync(int? nam);
}
