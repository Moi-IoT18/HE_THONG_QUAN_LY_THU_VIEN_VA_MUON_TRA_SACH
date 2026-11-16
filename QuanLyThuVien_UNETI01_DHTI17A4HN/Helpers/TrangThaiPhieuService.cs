// Họ và tên: Trịnh Thị Thanh Tâm
// Mã sinh viên: 23103100193
// Nội dung thực hiện: Bổ sung chuyển trạng thái hợp lệ cho duyệt và nhận trả phiếu mượn.

using System.ComponentModel.DataAnnotations;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class TrangThaiPhieuService
{
    public static bool DangXuLy(TrangThaiPhieu trangThai)
        => trangThai is TrangThaiPhieu.ChoDuyet or TrangThaiPhieu.DaDuyet or TrangThaiPhieu.DangMuon or TrangThaiPhieu.QuaHan;

    public static bool CoTheChuyen(TrangThaiPhieu hienTai, TrangThaiPhieu mucTieu)
    {
        return (hienTai, mucTieu) switch
        {
            (TrangThaiPhieu.ChoDuyet, TrangThaiPhieu.DaDuyet) => true,
            (TrangThaiPhieu.ChoDuyet, TrangThaiPhieu.DangMuon) => true,
            (TrangThaiPhieu.ChoDuyet, TrangThaiPhieu.DaHuy) => true,
            (TrangThaiPhieu.DaDuyet, TrangThaiPhieu.DangMuon) => true,
            (TrangThaiPhieu.DangMuon, TrangThaiPhieu.QuaHan) => true,
            (TrangThaiPhieu.DangMuon, TrangThaiPhieu.DaTra) => true,
            (TrangThaiPhieu.QuaHan, TrangThaiPhieu.DaTra) => true,
            _ => false,
        };
    }
}

public static class EnumDisplayExtensions
{
    public static string TenHienThi<T>(this T value) where T : Enum
    {
        var fi = value.GetType().GetField(value.ToString());
        var attr = fi?.GetCustomAttributes(typeof(DisplayAttribute), false).OfType<DisplayAttribute>().FirstOrDefault();
        return attr?.GetName() ?? value.ToString();
    }
}
