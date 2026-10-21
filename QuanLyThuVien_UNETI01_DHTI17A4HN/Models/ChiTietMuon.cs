// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity ChiTietMuon qu?n lý danh sách sách chi ti?t trong t?ng phi?u mu?n.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("ChiTietMuon")]
public class ChiTietMuon
{
    [Key]
    public int MaChiTietMuon { get; set; }

    [Display(Name = "Mã phi?u mu?n")]
    public int MaPhieuMuon { get; set; }
    [ForeignKey(nameof(MaPhieuMuon))]
    public PhieuMuon? PhieuMuon { get; set; }

    [Display(Name = "Mã sách")]
    public int MaSach { get; set; }
    [ForeignKey(nameof(MaSach))]
    public Sach? Sach { get; set; }

    [Display(Name = "S? lu?ng mu?n")]
    public int SoLuong { get; set; }

    [Display(Name = "Ðon giá")]
    public decimal DonGia { get; set; }

    [Display(Name = "Tr?ng thái tr?")]
    public bool DaTra { get; set; }
}
