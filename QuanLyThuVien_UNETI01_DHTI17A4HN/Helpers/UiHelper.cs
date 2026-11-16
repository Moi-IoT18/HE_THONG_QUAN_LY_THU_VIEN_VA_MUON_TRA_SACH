using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class UiHelper
{
    public static string Ngay(DateTime? value) => value.HasValue ? value.Value.ToString("dd/MM/yyyy") : "—";

    public static string Ngay(DateTime value) => value.ToString("dd/MM/yyyy");

    public static string NgayGio(DateTime? value) => value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : "—";

    public static string NgayGio(DateTime value) => value.ToString("dd/MM/yyyy HH:mm");

    public static string Tien(decimal? value)
    {
        if (value is null) return "0đ";
        return string.Format(CultureInfo.InvariantCulture, "{0:N0}đ", value.Value);
    }

    public static string MauTrangThai<TEnum>(TEnum value) where TEnum : Enum
    {
        return value switch
        {
            TrangThaiBanDoc.HoatDong => "bg-success",
            TrangThaiBanDoc.TamKhoa => "bg-warning text-dark",
            TrangThaiPhieu.ChoDuyet => "bg-secondary",
            TrangThaiPhieu.DaDuyet => "bg-primary",
            TrangThaiPhieu.DangMuon => "bg-info text-dark",
            TrangThaiPhieu.QuaHan => "bg-danger",
            TrangThaiPhieu.DaTra => "bg-success",
            TrangThaiPhieu.DaHuy => "bg-dark",
            _ => "bg-light text-dark",
        };
    }

    public static string MauTinhTrangTra(TinhTrangTra? value)
    {
        return value switch
        {
            TinhTrangTra.ChuaTra => "bg-secondary",
            TinhTrangTra.BinhThuong => "bg-success",
            TinhTrangTra.Hong => "bg-warning text-dark",
            TinhTrangTra.Mat => "bg-danger",
            null => "bg-secondary",
            _ => "bg-light text-dark",
        };
    }

    public static IEnumerable<SelectListItem> DanhSachEnum<TEnum>() where TEnum : struct, Enum
        => Enum.GetValues<TEnum>().Select(v => new SelectListItem(v.TenHienThi(), v.ToString()));

    public static IEnumerable<SelectListItem> DanhSachEnum<TEnum>(TEnum? selected) where TEnum : struct, Enum
    {
        var list = Enum.GetValues<TEnum>().Select(v => new SelectListItem(v.TenHienThi(), v.ToString(), EqualityComparer<TEnum>.Default.Equals(v, selected ?? default))).ToList();
        return list;
    }
}
