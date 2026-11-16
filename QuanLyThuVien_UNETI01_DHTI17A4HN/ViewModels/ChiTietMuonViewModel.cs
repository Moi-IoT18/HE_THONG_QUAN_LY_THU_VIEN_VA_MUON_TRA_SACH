// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Bộ lọc tra cứu chi tiết mượn sách bằng LINQ và phân trang.

using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public sealed class ChiTietMuonViewModel
{
    public string? TuKhoa { get; set; }
    public TrangThaiPhieu? TrangThai { get; set; }
    public int Trang { get; set; } = 1;
    public PhanTrang<ChiTietMuon>? DanhSach { get; set; }
}
