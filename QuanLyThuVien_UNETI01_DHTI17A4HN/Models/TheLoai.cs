// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Entity thể loại sách.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("TheLoai")]
public class TheLoai
{
    [Key]
    [Display(Name = "Mã thể loại")]
    public int MaTheLoai { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
    [StringLength(100, ErrorMessage = "Tên thể loại tối đa 100 ký tự")]
    [Display(Name = "Tên thể loại")]
    public string TenTheLoai { get; set; } = null!;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Trạng thái")]
    public bool TrangThai { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0")]
    [Display(Name = "Thứ tự hiển thị")]
    public int ThuTuHienThi { get; set; } = 1;

    public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
}
