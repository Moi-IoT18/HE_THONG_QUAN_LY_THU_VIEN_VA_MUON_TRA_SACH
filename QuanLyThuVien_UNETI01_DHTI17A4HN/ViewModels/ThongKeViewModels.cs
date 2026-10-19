// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: ViewModel cho chi tiết mượn, phiếu quá hạn, Dashboard và Thống kê (Module 5).
 
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
// ----------------------------- Chi tiết mượn -----------------------------
public class ChiTietMuonIndexViewModel
{
    public PhieuMuon Phieu { get; set; } = null!;
    public List<SelectListItem> DsSachCoTheThem { get; set; } = new();
    public List<string> CanhBao { get; set; } = new();
}
 
public class ChiTietMuonFormViewModel
{
    public int MaChiTiet { get; set; }
    public int MaPhieuMuon { get; set; }
    public string TenSach { get; set; } = "";
    public int SoLuongCon { get; set; }
 
    [Range(1, 100, ErrorMessage = "Số lượng phải lớn hơn 0")]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; } = 1;
 
    [Display(Name = "Tình trạng khi mượn")]
    public TinhTrangMuon TinhTrangMuon { get; set; }
 
    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}
 
// ----------------------------- Quá hạn -----------------------------
public class QuaHanIndexViewModel
{
    public string? TuKhoa { get; set; }
    public PhanTrang<PhieuQuaHanDong> DanhSach { get; set; } = new();
    public int TongPhieu { get; set; }
    public int TongCuon { get; set; }
    public decimal TongPhatDuKien { get; set; }
}
 
public class PhieuQuaHanDong
{
    public int MaPhieuMuon { get; set; }
    public int MaBanDoc { get; set; }
    public string HoTen { get; set; } = "";
    public string? SoDienThoai { get; set; }
    public DateTime HanTra { get; set; }
    public int SoCuon { get; set; }
    public List<string> TenSachs { get; set; } = new();
    public int SoNgayQuaHan => Services.QuaHanService.TinhSoNgayQuaHan(HanTra);
    public decimal PhatDuKien => SoNgayQuaHan * QuyDinh.PhatQuaHanMoiNgay * SoCuon;
}
 
// ----------------------------- Dashboard -----------------------------
public class DashboardViewModel
{
    public int TongTheLoai { get; set; }
    public int TongDauSach { get; set; }
    public int TongSoCuon { get; set; }
    public int SoSachKhaDung { get; set; }
    public int TongBanDoc { get; set; }
    public int TongGiaoDich { get; set; }
    public int SoChoXuLy { get; set; }
    public int SoDangXuLy { get; set; }
    public int SoQuaHan { get; set; }
    public int SoHoanThanh { get; set; }
    public decimal TongTienPhat { get; set; }
    public int SoPhieuVuaQuetQuaHan { get; set; }
 
    public List<PhieuMuon> PhieuChoDuyet { get; set; } = new();
    public List<PhieuMuon> PhieuQuaHanLauNhat { get; set; } = new();
}
 
// ----------------------------- Thống kê -----------------------------
public class ThongKeViewModel
{
    public int Nam { get; set; }
    public List<int> DsNam { get; set; } = new();
 
    public List<ThongKeDong> SachTheoTheLoai { get; set; } = new();
    public int[] LuotMuonTheoThang { get; set; } = new int[12];
    public List<ThongKeDong> SachMuonNhieuNhat { get; set; } = new();
    public List<ThongKeDong> BanDocMuonNhieuNhat { get; set; } = new();
    public List<Sach> SachChuaTungMuon { get; set; } = new();
 
    public int SoPhieuDangQuaHan { get; set; }
    public int SoPhieuTraTre { get; set; }
    public decimal TongTienPhat { get; set; }
    public List<ThongKeDong> TienPhatTheoLoai { get; set; } = new();
 
    public int SoPhieuDaTra { get; set; }
    public int SoPhieuTraDungHan { get; set; }
    public double TyLeTraDungHan => SoPhieuDaTra == 0 ? 0 : (double)SoPhieuTraDungHan / SoPhieuDaTra;
}
 
/// <summary>Một dòng thống kê dạng (nhãn, số lượng, giá trị phụ).</summary>
public class ThongKeDong
{
    public int Ma { get; set; }
    public string Ten { get; set; } = "";
    public string? MoTa { get; set; }
    public int SoLuong { get; set; }
    public decimal GiaTri { get; set; }
}
