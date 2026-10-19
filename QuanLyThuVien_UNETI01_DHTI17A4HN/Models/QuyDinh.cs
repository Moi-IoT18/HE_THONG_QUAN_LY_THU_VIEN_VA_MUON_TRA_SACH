// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Các tham s? quy d?nh nghi?p v? mu?n tr? (d?ng b? v?i sheet
//                     "Quy d?nh nghi?p v?" và docs/THIETKE_CSDL.md m?c 5).

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

public static class QuyDinh
{
    /// <summary>QD01 – S? ngày du?c mu?n tính t? ngày t?o phi?u.</summary>
    public const int SoNgayMuon = 14;

    /// <summary>QD02 – T?ng s? cu?n t?i da m?t b?n d?c du?c gi? (phi?u Ch? duy?t + Ðang mu?n + Quá h?n).</summary>
    public const int SoSachToiDa = 5;

    /// <summary>QD03 – Ti?n ph?t quá h?n cho m?i ngày, m?i cu?n (VNÐ).</summary>
    public const decimal PhatQuaHanMoiNgay = 5000m;

    /// <summary>QD04 – T? l? ph?t khi sách b? hu h?ng (so v?i don giá).</summary>
    public const decimal TyLePhatHuHong = 0.5m;

    /// <summary>QD05 – T? l? ph?t khi m?t sách (so v?i don giá).</summary>
    public const decimal TyLePhatMat = 1.0m;

    /// <summary>S? ngày tru?c h?n tr? du?c coi là "s?p d?n h?n" d? c?nh báo b?n d?c.</summary>
    public const int SoNgaySapDenHan = 2;

    /// <summary>S? dòng trên m?t trang danh sách.</summary>
    public const int KichThuocTrang = 10;
}
