// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Tiêu chí tìm kiếm, lọc và sắp xếp sách (Module 2).

using System.ComponentModel.DataAnnotations;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class SachFilterViewModel
{
    [StringLength(200)]
    [Display(Name = "Từ khóa")]
    public string? TuKhoa { get; set; }

    [Display(Name = "Thể loại")]
    public int? MaTheLoai { get; set; }

    [Range(1900, 2100)]
    [Display(Name = "Năm xuất bản")]
    public int? NamXuatBan { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiSach? TrangThai { get; set; }

    [Display(Name = "Tình trạng kho")]
    public bool? ConSach { get; set; }

    [Range(0, 100_000_000)]
    [Display(Name = "Giá từ")]
    public decimal? GiaTu { get; set; }

    [Range(0, 100_000_000)]
    [Display(Name = "Giá đến")]
    public decimal? GiaDen { get; set; }

    [Display(Name = "Sắp xếp")]
    public string SapXep { get; set; } = "ten_asc";
}
