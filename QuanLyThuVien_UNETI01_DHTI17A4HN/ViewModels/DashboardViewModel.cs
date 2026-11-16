// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Các chỉ số tổng quan và thống kê nổi bật cho Dashboard Module 5.

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public sealed class DashboardViewModel
{
    public int TongSoTheLoai { get; init; }
    public int TongSoSach { get; init; }
    public int SoSachKhaDung { get; init; }
    public int TongSoBanDoc { get; init; }
    public int TongSoGiaoDich { get; init; }
    public int SoGiaoDichChoXuLy { get; init; }
    public int SoGiaoDichDangXuLy { get; init; }
    public int SoGiaoDichHoanThanh { get; init; }
    public decimal TongTienPhat { get; init; }
    public decimal TongTienPhatDaNop { get; init; }
    public decimal TongTienPhatChuaNop { get; init; }
    public IReadOnlyList<LuotMuonTheoThangViewModel> LuotMuonTheoThang { get; init; } = [];
    public IReadOnlyList<SachTheoTheLoaiViewModel> SachTheoTheLoai { get; init; } = [];
    public IReadOnlyList<SachMuonNhieuViewModel> SachMuonNhieu { get; init; } = [];
}
