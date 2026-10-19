// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Interface truy cập dữ liệu Chi tiết mượn (luôn trong ngữ cảnh 1 phiếu).
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public interface IChiTietMuonRepository : IRepository<ChiTietMuon>
{
    /// <summary>Phiếu kèm bạn đọc và các dòng chi tiết (+ sách).</summary>
    Task<PhieuMuon?> LayPhieuKemChiTietAsync(int maPhieu, bool theoDoi);
 
    /// <summary>Sách còn phục vụ, còn hàng và chưa có trong phiếu.</summary>
    Task<List<Sach>> LaySachCoTheThemAsync(IEnumerable<int> maSachDaCo);
 
    /// <summary>1 dòng chi tiết kèm sách và phiếu (phiếu kèm toàn bộ chi tiết của nó).</summary>
    Task<ChiTietMuon?> LayKemPhieuAsync(int maChiTiet, bool theoDoi);
}
