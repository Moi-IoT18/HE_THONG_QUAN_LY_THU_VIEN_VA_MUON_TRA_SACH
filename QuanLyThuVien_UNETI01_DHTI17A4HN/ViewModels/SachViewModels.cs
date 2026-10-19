// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: ViewModel cho danh sách sách: điều kiện Tìm kiếm + Lọc + Sắp xếp + Phân trang,
//                     form sách, trang chủ và danh sách nhà xuất bản (Module 2).
 
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
public class SachLocViewModel
{
    [Display(Name = "Từ khóa")]
    public string? TuKhoa { get; set; }
 
    public string TimTheo { get; set; } = "tatca";
 
    public int? MaTheLoai { get; set; }
    public int? NamTu { get; set; }
    public int? NamDen { get; set; }
    public TrangThaiSach? TrangThai { get; set; }
    public bool? ConSach { get; set; }
    public decimal? GiaTu { get; set; }
    public decimal? GiaDen { get; set; }
    public string SapXep { get; set; } = "ten_asc";
    public int Trang { get; set; } = 1;
 
    public bool CoDieuKien =>
        !string.IsNullOrWhiteSpace(TuKhoa) || MaTheLoai.HasValue || NamTu.HasValue || NamDen.HasValue
        || TrangThai.HasValue || ConSach.HasValue || GiaTu.HasValue || GiaDen.HasValue;
 
    public static readonly List<SelectListItem> DsTimTheo =
    [
        new("Tất cả", "tatca"),
        new("Tên sách", "tensach"),
        new("Tên thể loại", "theloai"),
        new("Nhà xuất bản", "nxb"),
    ];
 
    public static readonly List<SelectListItem> DsSapXep =
    [
        new("Tên sách A → Z", "ten_asc"),
        new("Tên sách Z → A", "ten_desc"),
        new("Năm xuất bản tăng dần", "nam_asc"),
        new("Năm xuất bản giảm dần", "nam_desc"),
        new("Đơn giá tăng dần", "gia_asc"),
        new("Đơn giá giảm dần", "gia_desc"),
        new("Số lượng còn tăng dần", "con_asc"),
        new("Số lượng còn giảm dần", "con_desc"),
    ];
}
 
public class SachIndexViewModel
{
    public SachLocViewModel Loc { get; set; } = new();
    public PhanTrang<Sach> DanhSach { get; set; } = new();
    public List<SelectListItem> DsTheLoai { get; set; } = new();
    public HashSet<int> SachTrongGio { get; set; } = new();
}
 
public class SachFormViewModel
{
    public Sach Sach { get; set; } = new();
    public List<SelectListItem> DsTheLoai { get; set; } = new();
    public List<SelectListItem> DsNhaXuatBan { get; set; } = new();
    public int SoDangMuon { get; set; }
}
 
public class SachChiTietViewModel
{
    public Sach Sach { get; set; } = null!;
    public int SoLuotMuon { get; set; }
    public List<ChiTietMuon> LichSuMuon { get; set; } = new();
    public bool TrongGio { get; set; }
}
 
public class TrangChuViewModel
{
    public int TongDauSach { get; set; }
    public int TongTheLoai { get; set; }
    public int SoSachKhaDung { get; set; }
    public List<Sach> SachMoi { get; set; } = new();
    public List<SachMuonNhieu> SachMuonNhieu { get; set; } = new();
    public List<TheLoai> TheLoais { get; set; } = new();
}
 
public class SachMuonNhieu
{
    public int MaSach { get; set; }
    public string TenSach { get; set; } = "";
    public string? TacGia { get; set; }
    public int SoLuotMuon { get; set; }
}
 
public class NhaXuatBanIndexViewModel
{
    public string? TuKhoa { get; set; }
    public bool? TrangThai { get; set; }
    public PhanTrang<NhaXuatBanDong> DanhSach { get; set; } = new();
}
 
public class NhaXuatBanDong
{
    public int MaNhaXuatBan { get; set; }
    public string TenNhaXuatBan { get; set; } = "";
    public string? DiaChi { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
    public bool TrangThai { get; set; }
    public int SoDauSach { get; set; }
}
