// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: Entity TaiKhoan – tài kho?n dang nh?p c?a Admin, Th? thu, B?n d?c (Module 1).

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("TaiKhoan")]
public class TaiKhoan
{
    [Key]
    [Display(Name = "Mã tài kho?n")]
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Vui lòng nh?p tên dang nh?p")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên dang nh?p t? 3 d?n 50 ký t?")]
    [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "Tên dang nh?p ch? g?m ch? không d?u, s?, d?u _ ho?c .")]
    [Display(Name = "Tên dang nh?p")]
    public string TenDangNhap { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nh?p m?t kh?u")]
    [StringLength(255)]
    [Display(Name = "M?t kh?u")]
    public string MatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nh?p h? tên")]
    [StringLength(100, ErrorMessage = "H? tên t?i da 100 ký t?")]
    [Display(Name = "H? tên")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nh?p email")]
    [EmailAddress(ErrorMessage = "Email không dúng d?nh d?ng")]
    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Vai trò")]
    public VaiTro VaiTro { get; set; } = VaiTro.BanDoc;

    [Display(Name = "Ho?t d?ng")]
    public bool TrangThai { get; set; } = true;

    [Display(Name = "Ngày t?o")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    public BanDoc? BanDoc { get; set; }

    [InverseProperty(nameof(PhieuMuon.NhanVien))]
    public ICollection<PhieuMuon> PhieuMuonDaXuLy { get; set; } = new List<PhieuMuon>();
}
