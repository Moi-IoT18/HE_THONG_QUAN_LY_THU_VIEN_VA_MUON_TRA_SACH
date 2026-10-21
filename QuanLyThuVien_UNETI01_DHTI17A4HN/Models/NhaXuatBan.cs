// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity NhaXuatBan qu?n lý thông tin nhà xu?t b?n sách.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("NhaXuatBan")]
public class NhaXuatBan
{
    [Key]
    [Display(Name = "Mã NXB")]
    public int MaNhaXuatBan { get; set; }

    [Required(ErrorMessage = "Vui lòng nh?p tên nhà xu?t b?n")]
    [StringLength(150, ErrorMessage = "Tên NXB t?i da 150 ký t?")]
    [Display(Name = "Tên nhà xu?t b?n")]
    public string TenNhaXuatBan { get; set; } = null!;

    [StringLength(250)]
    [Display(Name = "Ð?a ch?")]
    public string? DiaChi { get; set; }

    [StringLength(20)]
    [Display(Name = "Ði?n tho?i")]
    public string? DienThoai { get; set; }

    public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
}
