// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Form nhập liệu và validation nhà xuất bản (Module 2).

using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class NhaXuatBanViewModel
{
    public int MaNhaXuatBan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên nhà xuất bản.")]
    [StringLength(150, ErrorMessage = "Tên nhà xuất bản tối đa 150 ký tự.")]
    [Display(Name = "Tên nhà xuất bản")]
    public string TenNhaXuatBan { get; set; } = "";

    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Đang hợp tác")]
    public bool TrangThai { get; set; } = true;
}
