// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Entity BanDoc – hồ sơ, thẻ bạn đọc (Module 3).
 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
 
[Table("BanDoc")]
public class BanDoc
{
    [Key]
    [Display(Name = "Mã bạn đọc")]
    public int MaBanDoc { get; set; }
 
    /// <summary>Tài khoản đăng nhập của bạn đọc (UNIQUE – quan hệ 1-1).</summary>
    [Display(Name = "Tài khoản")]
    public int MaTaiKhoan { get; set; }
 
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = null!;
 
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }
 
    [Display(Name = "Giới tính")]
    public GioiTinh? GioiTinh { get; set; }
 
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại gồm 10–11 chữ số, bắt đầu bằng 0")]
    [StringLength(15)]
    [Unicode(false)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }
 
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Email")]
    public string? Email { get; set; }
 
    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
 
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    [Display(Name = "Ngày cấp thẻ")]
    public DateTime NgayCapThe { get; set; } = DateTime.Today;
 
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    [Display(Name = "Hạn thẻ")]
    public DateTime HanThe { get; set; } = DateTime.Today.AddYears(1);
 
    [Display(Name = "Trạng thái")]
    public TrangThaiBanDoc TrangThai { get; set; } = TrangThaiBanDoc.HoatDong;
 
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
 
    // ----- Navigation Property -----
    [ForeignKey(nameof(MaTaiKhoan))]
    public TaiKhoan? TaiKhoan { get; set; }
 
    public ICollection<PhieuMuon> PhieuMuons { get; set; } = new List<PhieuMuon>();
 
    /// <summary>Thẻ còn hạn tính đến hôm nay.</summary>
    [NotMapped]
    public bool TheConHan => HanThe.Date >= DateTime.Today;
}
