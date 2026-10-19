// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Hàm hi?n th? dùng chung – tên ti?ng Vi?t c?a enum, màu badge theo tr?ng thái,
//                     d?nh d?ng ti?n/ngày.

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class UiHelper
{
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    public static string TenHienThi(this Enum giaTri)
    {
        var member = giaTri.GetType().GetMember(giaTri.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? giaTri.ToString();
    }

    public static List<SelectListItem> DanhSachEnum<TEnum>(TEnum? dangChon = null) where TEnum : struct, Enum
        => Enum.GetValues<TEnum>()
            .Select(v => new SelectListItem(v.TenHienThi(), v.ToString(), dangChon.HasValue && dangChon.Value.Equals(v)))
            .ToList();

    public static string Tien(decimal soTien) => soTien.ToString("N0", Vi) + " d";

    public static string Ngay(DateTime? ngay) => ngay?.ToString("dd/MM/yyyy") ?? "—";

    public static string NgayGio(DateTime? ngay) => ngay?.ToString("dd/MM/yyyy HH:mm") ?? "—";

    public static string MauTrangThai(TrangThaiPhieu tt) => tt switch
    {
        TrangThaiPhieu.ChoDuyet => "bg-warning text-dark",
        TrangThaiPhieu.DangMuon => "bg-primary",
        TrangThaiPhieu.QuaHan => "bg-danger",
        TrangThaiPhieu.DaTra => "bg-success",
        TrangThaiPhieu.DaHuy => "bg-secondary",
        TrangThaiPhieu.TuChoi => "bg-dark",
        _ => "bg-light text-dark",
    };

    public static string MauTrangThai(TrangThaiSach tt)
        => tt == TrangThaiSach.ConPhucVu ? "bg-success" : "bg-secondary";

    public static string MauTrangThai(TrangThaiBanDoc tt)
        => tt == TrangThaiBanDoc.HoatDong ? "bg-success" : "bg-secondary";

    public static string MauTinhTrangTra(TinhTrangTra? tt) => tt switch
    {
        TinhTrangTra.BinhThuong => "bg-success",
        TinhTrangTra.HuHong => "bg-warning text-dark",
        TinhTrangTra.Mat => "bg-danger",
        _ => "bg-light text-dark",
    };
}
