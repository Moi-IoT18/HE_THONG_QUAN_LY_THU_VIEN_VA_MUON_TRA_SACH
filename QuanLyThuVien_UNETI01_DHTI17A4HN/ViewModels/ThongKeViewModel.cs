// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: ViewModel tổng hợp kết quả truy vấn thống kê Module 5.

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public sealed class ThongKeViewModel
{
    public int SoPhieuQuaHan { get; init; }
    public decimal TongTienPhat { get; init; }
    public decimal TongTienPhatDaNop { get; init; }
    public decimal TongTienPhatChuaNop { get; init; }
    public int TongPhieuDaTra { get; init; }
    public int PhieuTraDungHan { get; init; }
    public decimal TyLeTraDungHan { get; init; }
    public IReadOnlyList<SachTheoTheLoaiViewModel> SachTheoTheLoai { get; init; } = [];
    public IReadOnlyList<LuotMuonTheoThangViewModel> LuotMuonTheoThang { get; init; } = [];
    public IReadOnlyList<SachMuonNhieuViewModel> SachMuonNhieu { get; init; } = [];
    public IReadOnlyList<BanDocMuonNhieuViewModel> BanDocMuonNhieu { get; init; } = [];
}

public sealed class SachTheoTheLoaiViewModel
{
    public string TenTheLoai { get; init; } = string.Empty;
    public int SoDauSach { get; init; }
    public int TongSoBan { get; init; }
}

public sealed class LuotMuonTheoThangViewModel
{
    public int Nam { get; init; }
    public int Thang { get; init; }
    public int SoLuotMuon { get; init; }
}

public sealed class SachMuonNhieuViewModel
{
    public int MaSach { get; init; }
    public string TenSach { get; init; } = string.Empty;
    public string? DuongDanAnhBia { get; init; }
    public int SoLuongMuon { get; init; }
}

public sealed class BanDocMuonNhieuViewModel
{
    public string HoTen { get; init; } = string.Empty;
    public int SoLuotMuon { get; init; }
}
