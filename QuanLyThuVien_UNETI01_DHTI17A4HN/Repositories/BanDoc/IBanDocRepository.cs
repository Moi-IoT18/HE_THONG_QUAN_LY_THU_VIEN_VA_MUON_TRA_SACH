// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Interface truy cập dữ liệu Bạn đọc.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public interface IBanDocRepository : IRepository<BanDoc>
{
    /// <summary>Danh sách bạn đọc: tìm kiếm, lọc trạng thái/tình trạng thẻ, sắp xếp, kèm số sách đang giữ.</summary>
    IQueryable<BanDocDong> TruyVanDanhSach(string? tuKhoa, TrangThaiBanDoc? trangThai, string? tinhTrangThe, string sapXep);
 
    /// <summary>Bạn đọc kèm tài khoản. theoDoi = true khi sắp sửa / xóa.</summary>
    Task<BanDoc?> LayKemTaiKhoanAsync(int maBanDoc, bool theoDoi = false);
 
    /// <summary>Hồ sơ của người đang đăng nhập (tìm theo mã tài khoản trong Session).</summary>
    Task<BanDoc?> LayTheoTaiKhoanAsync(int? maTaiKhoan);
 
    /// <summary>Lịch sử phiếu mượn của bạn đọc (kèm chi tiết + sách), mới nhất trước.</summary>
    Task<List<PhieuMuon>> LayLichSuPhieuAsync(int maBanDoc);
 
    /// <summary>Tổng số cuốn đang giữ (phiếu Đang mượn / Quá hạn).</summary>
    Task<int> DemSachDangGiuAsync(int maBanDoc);
 
    Task<int> DemPhieuAsync(int maBanDoc);
 
    Task<bool> TrungTenDangNhapAsync(string tenDangNhap);
 
    /// <summary>Xóa bạn đọc cùng tài khoản đăng nhập của họ.</summary>
    void XoaKemTaiKhoan(BanDoc banDoc);
}
