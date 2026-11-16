// Họ và tên: Trịnh Thị Thanh Tâm
// Mã sinh viên: 23103100193
// Nội dung thực hiện: Dữ liệu xác nhận tình trạng sách và nhận trả phiếu mượn (Module 4).
// Bổ sung Module 5: Phan Vũ Thanh Thư - 23103100316
// Nội dung thực hiện: Ghi nhận ghi chú kết quả nhận trả cho từng chi tiết mượn.

using System.ComponentModel.DataAnnotations;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

public class TraSachViewModel
{
    public int MaPhieuMuon { get; set; }
    public string HoTenBanDoc { get; set; } = "";
    public DateTime HanTra { get; set; }
    public int SoNgayQuaHan { get; set; }
    public decimal TienPhatTamTinh { get; set; }
    public List<TraSachDongViewModel> ChiTiet { get; set; } = new();
}

public class TraSachDongViewModel
{
    public int MaChiTiet { get; set; }
    public string TenSach { get; set; } = "";
    public string? DuongDanAnhBia { get; set; }
    public int SoLuong { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn tình trạng sách khi trả.")]
    [EnumDataType(typeof(TinhTrangTra), ErrorMessage = "Tình trạng trả không hợp lệ.")]
    public TinhTrangTra? TinhTrangTra { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
    public string? GhiChu { get; set; }
}
