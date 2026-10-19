using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("ChiTietMuon")]
public class ChiTietMuon
{
    [Key]
    public int MaChiTiet { get; set; }

    public int MaSach { get; set; }
    public int MaPhieuMuon { get; set; }

    [Range(1, 100)]
    public int SoLuong { get; set; } = 1;

    public TinhTrangMuon TinhTrangMuon { get; set; } = TinhTrangMuon.BinhThuong;

    public TinhTrangTra? TinhTrangTra { get; set; }

    public int SoNgayQuaHan { get; set; }

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue)]
    public decimal TienPhat { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [ForeignKey(nameof(MaSach))]
    public Sach? Sach { get; set; }

    [ForeignKey(nameof(MaPhieuMuon))]
    public PhieuMuon? PhieuMuon { get; set; }
}
