// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Khai báo các enum tr?ng thái/vai trò dùng chung cho toàn h? th?ng.
//                     T?t c? du?c luu xu?ng SQL Server d?ng chu?i (HasConversion<string>()).

using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

/// <summary>Vai trò c?a tài kho?n.</summary>
public enum VaiTro
{
    [Display(Name = "Qu?n tr? viên")] Admin,
    [Display(Name = "Th? thu")] ThuThu,
    [Display(Name = "B?n d?c")] BanDoc,
}

/// <summary>Tr?ng thái ph?c v? c?a m?t d?u sách.</summary>
public enum TrangThaiSach
{
    [Display(Name = "Còn ph?c v?")] ConPhucVu,
    [Display(Name = "Ng?ng ph?c v?")] NgungPhucVu,
}

/// <summary>Tr?ng thái th? b?n d?c.</summary>
public enum TrangThaiBanDoc
{
    [Display(Name = "Ho?t d?ng")] HoatDong,
    [Display(Name = "T?m khóa")] TamKhoa,
}

public enum GioiTinh
{
    [Display(Name = "Nam")] Nam,
    [Display(Name = "N?")] Nu,
    [Display(Name = "Khác")] Khac,
}

/// <summary>
/// Lu?ng tr?ng thái phi?u mu?n (m?c 8.4 c?a d?):
/// ChoDuyet (Ch? x? lý) ? DangMuon/QuaHan (Ðang x? lý) ? DaTra (Hoàn thành);
/// nhánh DaHuy / TuChoi.
/// </summary>
public enum TrangThaiPhieu
{
    [Display(Name = "Ch? duy?t")] ChoDuyet,
    [Display(Name = "Ðang mu?n")] DangMuon,
    [Display(Name = "Quá h?n")] QuaHan,
    [Display(Name = "Ðã tr?")] DaTra,
    [Display(Name = "Ðã h?y")] DaHuy,
    [Display(Name = "T? ch?i")] TuChoi,
}

/// <summary>Tình tr?ng sách lúc giao cho b?n d?c.</summary>
public enum TinhTrangMuon
{
    [Display(Name = "Bình thu?ng")] BinhThuong,
    [Display(Name = "Cu, nhàu")] CuNhung,
}

/// <summary>Tình tr?ng sách khi b?n d?c tr?.</summary>
public enum TinhTrangTra
{
    [Display(Name = "Bình thu?ng")] BinhThuong,
    [Display(Name = "Hu h?ng")] HuHong,
    [Display(Name = "M?t sách")] Mat,
}
