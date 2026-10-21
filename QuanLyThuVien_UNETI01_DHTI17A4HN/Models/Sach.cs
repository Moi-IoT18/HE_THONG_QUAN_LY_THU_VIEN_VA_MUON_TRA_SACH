// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity Sach qu?n lý kho sách trong thu vi?n.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("Sach")]
public class Sach
{
    [Key]
    [Display(Name = "Mã sách")]
    public int MaSach { get; set; }

    [Required(ErrorMessage = "Vui lòng nh?p tên sách")]
    [StringLength(200)]
    [Display(Name = "Tên sách")]
    public string TenSach { get; set; } = null!;

    [StringLength(100)]
    [Display(Name = "Tác gi?")]
    public string? TacGia { get; set; }

    [Display(Name = "Nam xu?t b?n")]
    public int NamXuatBan { get; set; }

    [Display(Name = "Giá ti?n")]
    [Range(0, 100000000)]
    public decimal GiaTien { get; set; }

    [Display(Name = "S? lu?ng t?n")]
    public int SoLuongTon { get; set; }

    [Display(Name = "Mã th? lo?i")]
    public int MaTheLoai { get; set; }
    [ForeignKey(nameof(MaTheLoai))]
    public TheLoai? TheLoai { get; set; }

    [Display(Name = "Mã NXB")]
    public int MaNhaXuatBan { get; set; }
    [ForeignKey(nameof(MaNhaXuatBan))]
    public NhaXuatBan? NhaXuatBan { get; set; }

    public ICollection<ChiTietMuon> ChiTietMuons { get; set; } = new List<ChiTietMuon>();
}
