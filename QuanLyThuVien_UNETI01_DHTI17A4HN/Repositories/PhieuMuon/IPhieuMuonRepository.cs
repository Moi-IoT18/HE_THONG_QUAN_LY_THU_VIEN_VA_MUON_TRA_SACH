// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Interface truy cập dữ liệu Phiếu mượn phía Bạn đọc.
//                     Mọi hàm "của bạn đọc" đều nhận maBanDoc để không xem/sửa được phiếu người khác.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public interface IPhieuMuonRepository : IRepository<PhieuMuon>
{
    /// <summary>Phiếu của một bạn đọc (kèm chi tiết + sách), mới nhất trước, có lọc trạng thái.</summary>
    IQueryable<PhieuMuon> TruyVanCuaBanDoc(int maBanDoc, TrangThaiPhieu? trangThai);
 
    /// <summary>Phiếu đã quá hạn hoặc sắp đến hạn (hạn trả &lt;= nguongSapHan).</summary>
    Task<List<PhieuMuon>> LayCanhBaoHanAsync(int maBanDoc, DateTime nguongSapHan);
 
    /// <summary>Chi tiết 1 phiếu (chỉ đọc) – chỉ trả về nếu đúng của bạn đọc này.</summary>
    Task<PhieuMuon?> LayChiTietCuaBanDocAsync(int maPhieu, int maBanDoc);
 
    /// <summary>Phiếu (có theo dõi để sửa) – chỉ trả về nếu đúng của bạn đọc này.</summary>
    Task<PhieuMuon?> TimCuaBanDocAsync(int maPhieu, int maBanDoc);
 
    /// <summary>Bạn đọc có phiếu quá hạn chưa trả không (bỏ qua 1 phiếu nếu cần).</summary>
    Task<bool> CoPhieuQuaHanAsync(int maBanDoc, int? maPhieuBoQua);
 
    /// <summary>Các phiếu đang "giữ" sách: Chờ duyệt / Đang mượn / Quá hạn.</summary>
    IQueryable<PhieuMuon> TruyVanPhieuDangGiu(int maBanDoc, int? maPhieuBoQua);
}
