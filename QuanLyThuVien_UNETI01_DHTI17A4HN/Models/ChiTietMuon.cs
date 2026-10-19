// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Entity ChiTietMuon – kết quả mượn/trả của từng đầu sách trong phiếu (Module 5).
 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
 
[Table("ChiTietMuon")]
public class ChiTietMuon
{
    [Key]
    [Display(Name = "Mã chi tiết")]
    public int MaChiTiet { get; set; }
 
    [Display(Name = "Phiếu mượn")]
    public int MaPhieuMuon { get; set; }
 
    [Required(ErrorMessage = "Vui lòng chọn sách")]
    [Display(Name = "Sách")]
    public int MaSach { get; set; }
 
    [Range(1, 100, ErrorMessage = "Số lượng phải lớn hơn 0")]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; } = 1;
 
    [Display(Name = "Tình trạng khi mượn")]
    public TinhTrangMuon TinhTrangMuon { get; set; } = TinhTrangMuon.BinhThuong;
 
    /// <summary>null = chưa trả.</summary>
    [Display(Name = "Tình trạng khi trả")]
    public TinhTrangTra? TinhTrangTra { get; set; }
 
    [Range(0, 3650, ErrorMessage = "Số ngày quá hạn không hợp lệ")]
    [Display(Name = "Số ngày quá hạn")]
    public int SoNgayQuaHan { get; set; }
 
    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue, ErrorMessage = "Tiền phạt không được âm")]
    [DisplayFormat(DataFormatString = "{0:N0} đ")]
    [Display(Name = "Tiền phạt")]
    public decimal TienPhat { get; set; }
 
    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
 
    // ----- Navigation Property -----
    [ForeignKey(nameof(MaPhieuMuon))]
    public PhieuMuon? PhieuMuon { get; set; }
 
    [ForeignKey(nameof(MaSach))]
    public Sach? Sach { get; set; }
}
