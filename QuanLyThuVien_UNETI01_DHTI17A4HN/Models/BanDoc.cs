// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity BanDoc qu?n lý thông tin chi ti?t b?n d?c mu?n sách.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("BanDoc")]
public class BanDoc
{
    [Key, ForeignKey(nameof(TaiKhoan))]
    [Display(Name = "Mã b?n d?c / tài kho?n")]
    public int MaBanDoc { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }

    [Required(ErrorMessage = "Vui lòng nh?p s? th?")]
    [StringLength(50)]
    [Display(Name = "S? th?")]
    public string SoThe { get; set; } = null!;

    [StringLength(200)]
    [Display(Name = "Ð?a ch?")]
    public string? DiaChi { get; set; }

    [StringLength(20)]
    [Display(Name = "S? di?n tho?i")]
    public string? DienThoai { get; set; }

    [Display(Name = "Ngày h?t h?n th?")]
    public DateTime NgayHetHan { get; set; }

    public ICollection<PhieuMuon> PhieuMuons { get; set; } = new List<PhieuMuon>();
}
