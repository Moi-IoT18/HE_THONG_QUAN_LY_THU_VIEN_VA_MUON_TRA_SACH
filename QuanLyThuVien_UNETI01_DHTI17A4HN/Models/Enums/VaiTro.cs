using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

public enum VaiTro
{
    [Display(Name = "Người dùng")]
    BanDoc = 1,

    [Display(Name = "Nhân viên quản lý")]
    ThuThu = 2,

    [Display(Name = "Admin")]
    QuanLy = 3
}
