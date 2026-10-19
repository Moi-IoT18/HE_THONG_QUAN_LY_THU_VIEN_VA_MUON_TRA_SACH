using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

public enum TrangThaiPhieu
{
    DangMuon = 1,
    DaTra = 2,
    QuaHan = 3,
    Huy = 4,
    ChoDuyet = 5
}

public class BanDoc
{
    [Key]
    public int MaBanDoc { get; set; }

    [StringLength(200)]
    public string? HoTen { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? SoDienThoai { get; set; }
}

[Table("PhieuMuon")]
public class PhieuMuon
{
    [Key]
    public int MaPhieuMuon { get; set; }

    public int? MaBanDoc { get; set; }

    [DataType(DataType.Date)]
    public DateTime? NgayMuon { get; set; }

    [DataType(DataType.Date)]
    public DateTime? HanTra { get; set; }

    [DataType(DataType.Date)]
    public DateTime? NgayTra { get; set; }

    public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.DangMuon;

    [ForeignKey(nameof(MaBanDoc))]
    public BanDoc? BanDoc { get; set; }

    public ICollection<ChiTietMuon> ChiTietMuons { get; set; } = new List<ChiTietMuon>();
}
