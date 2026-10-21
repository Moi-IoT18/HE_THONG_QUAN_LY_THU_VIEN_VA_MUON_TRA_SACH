// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Entity Sach – đối tượng chính của hệ thống (Module 2).
 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
 
[Table("Sach")]
public class Sach
{
    [Key]
    [Display(Name = "Mã sách")]
    public int MaSach { get; set; }
 
    [Required(ErrorMessage = "Vui lòng nhập tên sách")]
    [StringLength(200, ErrorMessage = "Tên sách tối đa 200 ký tự")]
    [Display(Name = "Tên sách")]
    public string TenSach { get; set; } = null!;
 
    [StringLength(150, ErrorMessage = "Tên tác giả tối đa 150 ký tự")]
    [Display(Name = "Tác giả")]
    public string? TacGia { get; set; }
 
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thể loại")]
    [Display(Name = "Thể loại")]
    public int MaTheLoai { get; set; }
 
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhà xuất bản")]
    [Display(Name = "Nhà xuất bản")]
    public int MaNhaXuatBan { get; set; }
 
    [Range(1900, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
    [Display(Name = "Năm xuất bản")]
    public int NamXuatBan { get; set; } = DateTime.Today.Year;
 
    [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10.000")]
    [Display(Name = "Tổng số lượng")]
    public int SoLuong { get; set; }
 
    [Range(0, 10000, ErrorMessage = "Số lượng còn phải từ 0 đến 10.000")]
    [Display(Name = "Số lượng còn")]
    public int SoLuongCon { get; set; }
 
    [Column(TypeName = "decimal(18,0)")]
    [Range(0, 100_000_000, ErrorMessage = "Đơn giá phải từ 0 đến 100.000.000")]
    [DisplayFormat(DataFormatString = "{0:N0} đ")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }
 
    [StringLength(50)]
    [Display(Name = "Vị trí kệ")]
    public string? ViTriKeSach { get; set; }
 
    [Display(Name = "Trạng thái")]
    public TrangThaiSach TrangThai { get; set; } = TrangThaiSach.ConPhucVu;
 
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
 
    // ----- Navigation Property -----
    [ForeignKey(nameof(MaTheLoai))]
    [Display(Name = "Thể loại")]
    public TheLoai? TheLoai { get; set; }
 
    [ForeignKey(nameof(MaNhaXuatBan))]
    [Display(Name = "Nhà xuất bản")]
    public NhaXuatBan? NhaXuatBan { get; set; }
 
    public ICollection<ChiTietMuon> ChiTietMuons { get; set; } = new List<ChiTietMuon>();
 
    [NotMapped]
    [Display(Name = "Khả dụng")]
    public bool KhaDung => TrangThai == TrangThaiSach.ConPhucVu && SoLuongCon > 0;
}