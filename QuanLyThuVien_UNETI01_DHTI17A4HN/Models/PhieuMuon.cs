using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("PhieuMuon")]
public class PhieuMuon
{
    [Key]
    [Display(Name = "Mã phiếu")]
    public int MaPhieuMuon { get; set; }

    [Display(Name = "Bạn đọc")]
    public int MaBanDoc { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    [Display(Name = "Ngày mượn")]
    public DateTime NgayMuon { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    [Display(Name = "Hạn trả")]
    public DateTime HanTra { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    [Display(Name = "Ngày trả")]
    public DateTime? NgayTra { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.ChoDuyet;

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue, ErrorMessage = "Tiền phạt không được âm")]
    [DisplayFormat(DataFormatString = "{0:N0} đ")]
    [Display(Name = "Tiền phạt")]
    public decimal TienPhat { get; set; }

    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [Display(Name = "Nhân viên xử lý")]
    public int? NhanVienXuLy { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    [Display(Name = "Ngày xử lý")]
    public DateTime? NgayXuLy { get; set; }

    [ForeignKey(nameof(MaBanDoc))]
    public BanDoc? BanDoc { get; set; }

    [ForeignKey(nameof(NhanVienXuLy))]
    public User? NhanVien { get; set; }

    public ICollection<ChiTietMuon> ChiTietMuons { get; set; } = new List<ChiTietMuon>();
}
