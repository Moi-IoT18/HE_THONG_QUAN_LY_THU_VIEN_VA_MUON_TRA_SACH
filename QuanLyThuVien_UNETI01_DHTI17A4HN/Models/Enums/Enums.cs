using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

public enum VaiTro
{
    [Display(Name = "Quản trị viên")] Admin,
    [Display(Name = "Thủ thư")] ThuThu,
    [Display(Name = "Bạn đọc")] BanDoc,
}

public static class Quyen
{
    public const string Admin = "Admin";
    public const string ThuThu = "ThuThu";
    public const string BanDoc = "BanDoc";
    public const string NhanVien = "ThuThu,Admin";
}

public enum TrangThaiSach
{
    [Display(Name = "Còn phục vụ")] ConPhucVu = 1,
    [Display(Name = "Hết sách")] HetSach = 2,
    [Display(Name = "Ngừng phục vụ")] NgungPhucVu = 3,
}

public enum TrangThaiBanDoc
{
    [Display(Name = "Hoạt động")] HoatDong,
    [Display(Name = "Tạm khóa")] TamKhoa,
}

public enum GioiTinh
{
    [Display(Name = "Nam")] Nam,
    [Display(Name = "Nữ")] Nu,
    [Display(Name = "Khác")] Khac,
}

public enum TrangThaiPhieu
{
    [Display(Name = "Chờ duyệt")] ChoDuyet,
    [Display(Name = "Đang mượn")] DangMuon,
    [Display(Name = "Quá hạn")] QuaHan,
    [Display(Name = "Đã trả")] DaTra,
    [Display(Name = "Đã hủy")] DaHuy,
    [Display(Name = "Từ chối")] TuChoi,
}

public enum TinhTrangMuon
{
    [Display(Name = "Bình thường")] BinhThuong,
    [Display(Name = "Cũ, nhàu")] CuNhung,
}

public enum TinhTrangTra
{
    [Display(Name = "Bình thường")] BinhThuong,
    [Display(Name = "Hư hỏng")] HuHong,
    [Display(Name = "Mất sách")] Mat,
}
