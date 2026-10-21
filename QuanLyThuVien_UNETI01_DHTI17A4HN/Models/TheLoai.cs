// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity TheLoai – d? li?u n?n th? lo?i sách (Module 1).

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("TheLoai")]
public class TheLoai
{
    [Key]
    [Display(Name = "Mã th? lo?i")]
    public int MaTheLoai { get; set; }

    [Required(ErrorMessage = "Vui lòng nh?p tên th? lo?i")]
    [StringLength(100, ErrorMessage = "Tên th? lo?i t?i da 100 ký t?")]
    [Display(Name = "Tên th? lo?i")]
    public string TenTheLoai { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Mô t? t?i da 500 ký t?")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Mô t?")]
    public string? MoTa { get; set; }

    [Display(Name = "Ðang s? d?ng")]
    public bool TrangThai { get; set; } = true;

    [Range(0, 1000, ErrorMessage = "Th? t? hi?n th? t? 0 d?n 1000")]
    [Display(Name = "Th? t? hi?n th?")]
    public int ThuTuHienThi { get; set; }

    public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
}
