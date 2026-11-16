// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Form nhập liệu và dữ liệu phân trang sách (Module 2).

using System.ComponentModel.DataAnnotations;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class SachViewModel
{
    public int MaSach { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sách.")]
    [StringLength(200, ErrorMessage = "Tên sách tối đa 200 ký tự.")]
    [Display(Name = "Tên sách")]
    public string TenSach { get; set; } = "";

    [StringLength(150, ErrorMessage = "Tên tác giả tối đa 150 ký tự.")]
    [Display(Name = "Tác giả")]
    public string? TacGia { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thể loại.")]
    [Display(Name = "Thể loại")]
    public int MaTheLoai { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhà xuất bản.")]
    [Display(Name = "Nhà xuất bản")]
    public int MaNhaXuatBan { get; set; }

    [Range(1900, 2100, ErrorMessage = "Năm xuất bản không hợp lệ.")]
    [Display(Name = "Năm xuất bản")]
    public int NamXuatBan { get; set; } = DateTime.Today.Year;

    [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10.000.")]
    [Display(Name = "Tổng số lượng")]
    public int SoLuong { get; set; }

    [Range(0, 10000, ErrorMessage = "Số lượng còn phải từ 0 đến 10.000.")]
    [Display(Name = "Số lượng còn")]
    public int SoLuongCon { get; set; }

    [Range(0, 100_000_000, ErrorMessage = "Đơn giá phải từ 0 đến 100.000.000.")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [StringLength(50)]
    [Display(Name = "Vị trí kệ sách")]
    public string? ViTriKeSach { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiSach TrangThai { get; set; } = TrangThaiSach.ConPhucVu;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [Display(Name = "Ảnh bìa (PNG, JPEG hoặc WebP; tối đa 5 MB)")]
    public IFormFile? AnhBia { get; set; }

    public string? DuongDanAnhBiaHienTai { get; set; }

    [Display(Name = "Xóa ảnh bìa hiện tại")]
    public bool XoaAnhBia { get; set; }
}

public class SachDanhSachViewModel
{
    public SachFilterViewModel Filter { get; set; } = new();
    public PhanTrang<Sach> DanhSach { get; set; } = new();
}
