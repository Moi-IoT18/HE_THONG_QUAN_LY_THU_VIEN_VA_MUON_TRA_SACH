// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Enum trạng thái sách.

using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

public enum TrangThaiSach
{
    [Display(Name = "Còn phục vụ")]
    ConPhucVu = 1,
    [Display(Name = "Ngừng phục vụ")]
    NgungPhucVu = 0
}