using System.ComponentModel.DataAnnotations;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class TaiKhoanCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự.")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Vai trò")]
    public VaiTro VaiTro { get; set; } = VaiTro.BanDoc;

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}

public class TaiKhoanEditViewModel
{
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại (nếu đổi mật khẩu)")]
    public string? MatKhauCu { get; set; }

    [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string? MatKhauMoi { get; set; }

    [Display(Name = "Vai trò")]
    public VaiTro VaiTro { get; set; } = VaiTro.BanDoc;

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}
