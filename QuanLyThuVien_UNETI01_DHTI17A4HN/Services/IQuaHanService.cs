// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Hợp đồng quét quá hạn và lấy danh sách phiếu quá hạn (Module 5).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public interface IQuaHanService
{
    Task QuetQuaHanAsync();
    Task<List<PhieuMuon>> LayDanhSachQuaHanAsync();
}
