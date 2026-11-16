// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: ViewModel cho quản lý bạn đọc, hồ sơ cá nhân, giỏ mượn và theo dõi phiếu (Module 3).
 
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
public class BanDocIndexViewModel
{
    public string? TuKhoa { get; set; }
    public TrangThaiBanDoc? TrangThai { get; set; }
 
    /// <summary>conhan | hethan</summary>
    public string? TinhTrangThe { get; set; }
 
    /// <summary>ten_asc | ten_desc | ngaycap_desc | hanthe_asc</summary>
    public string SapXep { get; set; } = "ten_asc";
 
    public PhanTrang<BanDocDong> DanhSach { get; set; } = new();
}
 
public class BanDocDong
{
    public int MaBanDoc { get; set; }
    public string HoTen { get; set; } = "";
    public string TenDangNhap { get; set; } = "";
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
    public DateTime NgayCapThe { get; set; }
    public DateTime HanThe { get; set; }
    public TrangThaiBanDoc TrangThai { get; set; }
    public bool TaiKhoanHoatDong { get; set; }
    public int SoSachDangGiu { get; set; }
}
 
/// <summary>Form Thủ thư thêm/sửa bạn đọc (khi thêm sẽ tạo luôn tài khoản đăng nhập).</summary>
public class BanDocFormViewModel
{
    public int MaBanDoc { get; set; }
 
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "Chỉ gồm chữ không dấu, số, dấu _ hoặc .")]
    [Display(Name = "Tên đăng nhập")]
    public string? TenDangNhap { get; set; }
 
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string? MatKhau { get; set; }
 
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = "";
 
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }
 
    [Display(Name = "Giới tính")]
    public GioiTinh? GioiTinh { get; set; }
 
    [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại gồm 10–11 chữ số, bắt đầu bằng 0")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }
 
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";
 
    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
 
    [DataType(DataType.Date)]
    [Display(Name = "Ngày cấp thẻ")]
    public DateTime NgayCapThe { get; set; } = DateTime.Today;
 
    [DataType(DataType.Date)]
    [Display(Name = "Hạn thẻ")]
    public DateTime HanThe { get; set; } = DateTime.Today.AddYears(1);
 
    [Display(Name = "Trạng thái thẻ")]
    public TrangThaiBanDoc TrangThai { get; set; } = TrangThaiBanDoc.HoatDong;
 
    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}
 
/// <summary>Bạn đọc tự cập nhật hồ sơ – KHÔNG có hạn thẻ, trạng thái (chỉ thủ thư sửa).</summary>
public class HoSoViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = "";
 
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }
 
    [Display(Name = "Giới tính")]
    public GioiTinh? GioiTinh { get; set; }
 
    [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại gồm 10–11 chữ số, bắt đầu bằng 0")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }
 
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";
 
    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
 
    // ----- Chỉ hiển thị -----
    public int MaBanDoc { get; set; }
    [ValidateNever]
    public string TenDangNhap { get; set; } = "";
    public DateTime NgayCapThe { get; set; }
    public DateTime HanThe { get; set; }
    public TrangThaiBanDoc TrangThai { get; set; }
    public int SoSachDangGiu { get; set; }
}
 
public class DoiMatKhauViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string MatKhauCu { get; set; } = "";
 
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string MatKhauMoi { get; set; } = "";
 
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu nhập lại không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu mới")]
    public string XacNhan { get; set; } = "";
}
 
public class BanDocChiTietViewModel
{
    public BanDoc BanDoc { get; set; } = null!;
    public List<PhieuMuon> PhieuMuons { get; set; } = new();
    public int SoSachDangGiu { get; set; }
    public decimal TongTienPhat { get; set; }
}
 
public class GioMuonViewModel
{
    public List<Sach> Sachs { get; set; } = new();
 
    /// <summary>Kết quả kiểm tra điều kiện trước khi tạo phiếu (để bạn đọc biết trước).</summary>
    public List<string> CanhBao { get; set; } = new();
 
    public DateTime HanTraDuKien => DateTime.Today.AddDays(QuyDinh.SoNgayMuon);
}
 
public class PhieuCuaToiViewModel
{
    public TrangThaiPhieu? TrangThai { get; set; }
    public PhanTrang<PhieuMuon> DanhSach { get; set; } = new();
 
    /// <summary>Phiếu đang mượn sắp đến hạn hoặc đã quá hạn – hiển thị cảnh báo (SV5).</summary>
    public List<PhieuMuon> CanhBaoHan { get; set; } = new();
}
