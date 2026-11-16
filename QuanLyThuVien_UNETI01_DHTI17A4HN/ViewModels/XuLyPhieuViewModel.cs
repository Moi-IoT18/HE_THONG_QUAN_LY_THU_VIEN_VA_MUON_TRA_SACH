// Họ và tên: Trịnh Thị Thanh Tâm
// Mã sinh viên: 23103100193
// Nội dung thực hiện: Bộ lọc danh sách và dữ liệu chi tiết xử lý phiếu mượn (Module 4).

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class XuLyPhieuFilterViewModel
{
    [StringLength(100)]
    public string? TuKhoa { get; set; }

    public int? MaTheLoai { get; set; }
    public int? MaNhaXuatBan { get; set; }
    public TrangThaiPhieu? TrangThai { get; set; }
    [DataType(DataType.Date)]
    public DateTime? TuNgay { get; set; }
    [DataType(DataType.Date)]
    public DateTime? DenNgay { get; set; }
    public string SapXep { get; set; } = "ngay_desc";
    public int Trang { get; set; } = 1;
    [BindNever]
    public PhanTrang<PhieuMuon> DanhSach { get; set; } = new();
}

public class XuLyPhieuViewModel
{
    public PhieuMuon PhieuMuon { get; set; } = null!;
}
