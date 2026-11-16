// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Entity chi tiết mượn sách – từng quyển sách trong một phiếu mượn.
// Bổ sung Module 5: Phan Vũ Thanh Thư - 23103100316
// Nội dung thực hiện: Ràng buộc miền giá trị kết quả trả sách, số ngày quá hạn và tiền phạt.
// Bổ sung validation khóa ngoại và enum Module 5: Phan Vũ Thanh Thư - 23103100316.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

[Table("ChiTietMuon")]
public class ChiTietMuon
{
    [Key]
    [Display(Name = "Mã chi tiết")]
    public int MaChiTiet { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Phiếu mượn không hợp lệ")]
    [Display(Name = "Mã phiếu mượn")]
    public int MaPhieuMuon { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sách không hợp lệ")]
    [Display(Name = "Mã sách")]
    public int MaSach { get; set; }

    [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 đến 1000")]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; } = 1;

    [EnumDataType(typeof(TinhTrangMuon), ErrorMessage = "Tình trạng mượn không hợp lệ")]
    [Display(Name = "Tình trạng mượn")]
    public TinhTrangMuon TinhTrangMuon { get; set; } = TinhTrangMuon.BinhThuong;

    [Display(Name = "Tình trạng trả")]
    public TinhTrangTra? TinhTrangTra { get; set; }

    [Range(0, 36500, ErrorMessage = "Số ngày quá hạn phải từ 0 đến 36.500")]
    [Display(Name = "Số ngày quá hạn")]
    public int SoNgayQuaHan { get; set; }

    [Column(TypeName = "decimal(18,0)")]
    [Range(typeof(decimal), "0", "1000000000000", ErrorMessage = "Tiền phạt phải từ 0 đến 1.000.000.000.000")]
    [Display(Name = "Tiền phạt")]
    public decimal TienPhat { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [ForeignKey(nameof(MaPhieuMuon))]
    public PhieuMuon? PhieuMuon { get; set; }

    [ForeignKey(nameof(MaSach))]
    public Sach? Sach { get; set; }
}
