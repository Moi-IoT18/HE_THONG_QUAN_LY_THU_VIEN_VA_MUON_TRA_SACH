// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Interface xử lý quá hạn: quét phiếu quá hạn, danh sách phiếu quá hạn.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public interface IQuaHanService
{
    /// <summary>Chuyển các phiếu Đang mượn đã qua hạn trả sang Quá hạn. Trả về số phiếu vừa cập nhật.</summary>
    Task<int> QuetQuaHanAsync();
 
    /// <summary>Danh sách phiếu quá hạn (lâu nhất lên đầu) kèm tổng số phiếu, số cuốn, tiền phạt dự kiến.</summary>
    Task<QuaHanIndexViewModel> LayDanhSachAsync(string? tuKhoa, int trang);
}
