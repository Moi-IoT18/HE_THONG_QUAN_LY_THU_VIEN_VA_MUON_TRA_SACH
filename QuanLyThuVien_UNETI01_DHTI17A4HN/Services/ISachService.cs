// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Hợp đồng nghiệp vụ danh sách và CRUD sách (Module 2).

using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public interface ISachService
{
    Task<SachDanhSachViewModel> LayDanhSachSachAsync(SachFilterViewModel filter, int trang);
    Task<Models.Sach?> LaySachTheoIdAsync(int id);
    Task ThemSachAsync(Models.Sach sach);
    Task CapNhatSachAsync(Models.Sach sach);
    Task<bool> KhoaNgoaiHopLeAsync(int maTheLoai, int maNhaXuatBan, bool choPhepNgungHoatDong = false);
    Task<bool> XoaSachAsync(int id);
}
