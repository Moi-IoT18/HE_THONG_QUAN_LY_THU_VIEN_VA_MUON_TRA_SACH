// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Entity NhaXuatBan – dữ liệu nền nhà xuất bản (Module 2).
 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
 
[Table("NhaXuatBan")]
public class NhaXuatBan
{
    [Key]
    [Display(Name = "Mã NXB")]
    public int MaNhaXuatBan { get; set; }
 
    [Required(ErrorMessage = "Vui lòng nhập tên nhà xuất bản")]
    [StringLength(150, ErrorMessage = "Tên nhà xuất bản tối đa 150 ký tự")]
    [Display(Name = "Tên nhà xuất bản")]
    public string TenNhaXuatBan { get; set; } = null!;
 
    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
 
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Unicode(false)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }
 
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Email")]
    public string? Email { get; set; }
 
    [Display(Name = "Đang hợp tác")]
    public bool TrangThai { get; set; } = true;
 
    // ----- Navigation Property -----
    public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
}
