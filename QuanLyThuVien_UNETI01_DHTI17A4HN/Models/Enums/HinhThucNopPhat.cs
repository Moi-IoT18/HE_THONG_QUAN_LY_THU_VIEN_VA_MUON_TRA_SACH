using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

public enum HinhThucNopPhat
{
    [Display(Name = "Tiền mặt tại quầy")]
    TienMat = 1,

    [Display(Name = "Chuyển khoản")]
    ChuyenKhoan = 2,
}
