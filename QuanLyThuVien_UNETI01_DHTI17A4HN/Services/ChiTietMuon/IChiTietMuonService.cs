// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Interface nghiệp vụ Chi tiết mượn.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public interface IChiTietMuonService
{
    /// <summary>Các dòng chi tiết của 1 phiếu + danh sách sách có thể thêm. null nếu không có phiếu.</summary>
    Task<ChiTietMuonIndexViewModel?> LayDanhSachAsync(int maPhieu);
 
    Task<ServiceResult> ThemAsync(int maPhieu, int maSach, int soLuong);
 
    /// <summary>
    /// Form sửa. Tìm thấy thì DuLieu luôn có (để biết MaPhieuMuon);
    /// phiếu không còn "Chờ duyệt" thì kèm lỗi.
    /// </summary>
    Task<ServiceResult<ChiTietMuonFormViewModel>> LayFormSuaAsync(int id);
 
    Task<ServiceResult> CapNhatAsync(int id, ChiTietMuonFormViewModel vm);
 
    /// <summary>Bỏ sách khỏi phiếu. DuLieu = mã phiếu (để quay lại trang phiếu).</summary>
    Task<ServiceResult<int>> XoaAsync(int id);
}
