// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Interface kiểm tra điều kiện mượn (dùng chung cho SV3 và SV4).
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
/// <summary>Một dòng sách muốn mượn.</summary>
public record SachYeuCau(int MaSach, int SoLuong);
 
public interface IMuonSachService
{
    /// <summary>
    /// Trả về danh sách lỗi (rỗng = đủ điều kiện).
    /// </summary>
    /// <param name="maBanDoc">Bạn đọc mượn.</param>
    /// <param name="dsSach">Các sách muốn mượn.</param>
    /// <param name="maPhieuBoQua">Khi thủ thư duyệt: bỏ qua chính phiếu đang duyệt khi đếm trùng / đếm giới hạn.</param>
    Task<List<string>> KiemTraDieuKienAsync(int maBanDoc, IReadOnlyCollection<SachYeuCau> dsSach, int? maPhieuBoQua = null);
}
