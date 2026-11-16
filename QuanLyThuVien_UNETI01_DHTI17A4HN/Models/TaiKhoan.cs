// Họ và tên: Nguyễn Thế Đông
// Mã sinh viên: 23103100187
// Nội dung thực hiện: Entity tài khoản hệ thống – dùng cho đăng nhập và phân quyền.
// Bổ sung Module 5: Phan Vũ Thanh Thư - 23103100316: validation enum vai trò.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("TaiKhoan")]
public class TaiKhoan
{
    [Key]
    [Display(Name = "Mã tài khoản")]
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự")]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [EnumDataType(typeof(VaiTro), ErrorMessage = "Vai trò không hợp lệ")]
    [Display(Name = "Vai trò")]
    public VaiTro VaiTro { get; set; } = VaiTro.BanDoc;

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;

    [Display(Name = "Ngày tạo")]
    public DateTime? NgayTao { get; set; }

    public BanDoc? BanDoc { get; set; }

    public ICollection<PhieuMuon> PhieuMuonsXuLy { get; set; } = new List<PhieuMuon>();
}
