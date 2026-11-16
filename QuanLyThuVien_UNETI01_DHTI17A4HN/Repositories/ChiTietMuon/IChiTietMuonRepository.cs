// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Truy vấn danh sách và chi tiết mượn sách có kiểm soát quan hệ.

using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using ChiTietMuonModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.ChiTietMuon;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.ChiTietMuon;

public interface IChiTietMuonRepository
{
    IQueryable<ChiTietMuonModel> TruyVanDanhSach(string? tuKhoa, TrangThaiPhieu? trangThai);
    Task<ChiTietMuonModel?> LayChiTietAsync(int maChiTiet);
}
