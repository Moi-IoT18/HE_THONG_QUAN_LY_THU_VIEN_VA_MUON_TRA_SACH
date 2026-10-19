using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("TheLoai")]
public class TheLoai
{
    [Key]
    public int MaTheLoai { get; set; }

    [Required]
    [StringLength(100)]
    public string TenTheLoai { get; set; } = string.Empty;

    [StringLength(255)]
    public string? MoTa { get; set; }

    public int ThuTuHienThi { get; set; }

    public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
}
