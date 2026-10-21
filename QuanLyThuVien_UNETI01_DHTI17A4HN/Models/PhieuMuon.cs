// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity PhieuMuon qu?n lý phi?u mu?n sách c?a b?n d?c.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("PhieuMuon")]
public class PhieuMuon
{
    [Key]
    [Display(Name = "Mã phi?u mu?n")]
    public int MaPhieuMuon { get; set; }

    [Display(Name = "Mã b?n d?c")]
    public int MaBanDoc { get; set; }
    [ForeignKey(nameof(MaBanDoc))]
    public BanDoc? BanDoc { get; set; }

    [Display(Name = "Mã nhân viên l?p phi?u")]
    public int MaNhanVien { get; set; }
    [ForeignKey(nameof(MaNhanVien))]
    public TaiKhoan? NhanVien { get; set; }

    [Display(Name = "Ngày mu?n")]
    public DateTime NgayMuon { get; set; } = DateTime.Now;

    [Display(Name = "H?n tr?")]
    public DateTime HanTra { get; set; }

    [Display(Name = "Tr?ng thái phi?u")]
    public int TrangThai { get; set; } // 0: Ðang mu?n, 1: Ðã tr?, 2: Quá h?n

    public ICollection<ChiTietMuon> ChiTietMuons { get; set; } = new List<ChiTietMuon>();
}
