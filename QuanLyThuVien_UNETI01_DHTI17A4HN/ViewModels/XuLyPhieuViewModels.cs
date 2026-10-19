// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: ViewModel cho danh sách phi?u c?n x? lý (tìm/l?c/s?p x?p) và form tr? sách (Module 4).

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class PhieuMuonLocViewModel
{
    public string? BanDoc { get; set; }
    public string? TenSach { get; set; }
    public int? MaTheLoai { get; set; }
    public TrangThaiPhieu? TrangThai { get; set; }
    [DataType(DataType.Date)] public DateTime? TuNgay { get; set; }
    [DataType(DataType.Date)] public DateTime? DenNgay { get; set; }
    public string SapXep { get; set; } = "ngay_desc";
    public int Trang { get; set; } = 1;
}

public class XuLyPhieuIndexViewModel
{
    public PhieuMuonLocViewModel Loc { get; set; } = new();
    public PhanTrang<PhieuMuon> DanhSach { get; set; } = new();
    public List<SelectListItem> DsTheLoai { get; set; } = new();
    public Dictionary<TrangThaiPhieu, int> DemTheoTrangThai { get; set; } = new();
}

public class TraSachViewModel
{
    public int MaPhieuMuon { get; set; }
    [ValidateNever] public PhieuMuon? Phieu { get; set; }
    public int SoNgayQuaHan { get; set; }
    public List<TraSachDong> Dong { get; set; } = new();
    [StringLength(300)] [Display(Name = "Ghi chú khi nh?n tr?")] public string? GhiChu { get; set; }
}

public class TraSachDong
{
    public int MaChiTiet { get; set; }
    [ValidateNever] public string TenSach { get; set; } = "";
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    [Display(Name = "Tình tr?ng tr?")] public TinhTrangTra TinhTrangTra { get; set; } = TinhTrangTra.BinhThuong;
    [StringLength(200)] public string? GhiChu { get; set; }
}
