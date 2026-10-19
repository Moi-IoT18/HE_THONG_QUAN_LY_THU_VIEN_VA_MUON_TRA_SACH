using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("ChiTietMuon")]
public class ChiTietMuon
{
    [Key]
    public int MaChiTietMuon { get; set; }

    public int MaSach { get; set; }
    public int MaPhieuMuon { get; set; }

    [ForeignKey(nameof(MaSach))]
    public Sach? Sach { get; set; }

    [ForeignKey(nameof(MaPhieuMuon))]
    public PhieuMuon? PhieuMuon { get; set; }
}
