using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

public enum TinhTrangTra
{
    [Display(Name = "Chưa trả")]
    ChuaTra = 0,

    [Display(Name = "Bình thường")]
    BinhThuong = 1,

    [Display(Name = "Hỏng")]
    Hong = 2,

    [Display(Name = "Mất")]
    Mat = 3
}
