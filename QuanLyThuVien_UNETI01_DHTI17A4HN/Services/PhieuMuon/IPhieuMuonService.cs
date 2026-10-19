// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Interface nghiệp vụ phía Bạn đọc: giỏ mượn, đăng ký mượn, theo dõi, hủy phiếu.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public interface IPhieuMuonService
{
    /// <summary>Giỏ mượn + cảnh báo nếu chưa đủ điều kiện mượn.</summary>
    Task<GioMuonViewModel> LayGioMuonAsync(int maBanDoc, List<int> gio);
 
    /// <summary>Thêm sách vào giỏ (danh sách gio được sửa trực tiếp, Controller lưu lại Session).</summary>
    Task<ServiceResult> ThemVaoGioAsync(int maSach, List<int> gio);
 
    /// <summary>Tạo phiếu "Chờ duyệt" từ giỏ. DuLieu = mã phiếu mới.</summary>
    Task<ServiceResult<int>> TaoPhieuAsync(int maBanDoc, List<int> gio, string? ghiChu);
 
    Task<PhieuCuaToiViewModel> LayPhieuCuaToiAsync(int maBanDoc, TrangThaiPhieu? trangThai, int trang);
 
    Task<PhieuMuon?> LayChiTietAsync(int maPhieu, int maBanDoc);
 
    /// <summary>Chỉ hủy được khi phiếu còn "Chờ duyệt".</summary>
    Task<ServiceResult> HuyAsync(int maPhieu, int maBanDoc);
}
