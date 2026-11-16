using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

public enum TrangThaiPhieu
{
    [Display(Name = "Chờ duyệt")]
    ChoDuyet = 1,

    [Display(Name = "Đã duyệt")]
    DaDuyet = 2,

    [Display(Name = "Đang mượn")]
    DangMuon = 3,

    [Display(Name = "Quá hạn")]
    QuaHan = 4,

    [Display(Name = "Đã trả")]
    DaTra = 5,

    [Display(Name = "Đã hủy")]
    DaHuy = 6
}
