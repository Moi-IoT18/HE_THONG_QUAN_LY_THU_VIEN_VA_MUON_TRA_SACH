using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class UiHelper
{
    public static string Tien(decimal value) => value.ToString("N0") + " đ";

    public static string Ngay(DateTime? value) => value?.ToString("dd/MM/yyyy") ?? "-";

    public static string MauTrangThai(Enum value) => value switch
    {
        TrangThaiSach.ConPhucVu => "bg-success",
        TrangThaiSach.HetSach => "bg-warning text-dark",
        TrangThaiSach.NgungPhucVu => "bg-secondary",
        _ => "bg-light text-dark"
    };

    public static string TenHienThi(this Enum value) => value switch
    {
        TrangThaiSach.ConPhucVu => "Còn phục vụ",
        TrangThaiSach.HetSach => "Hết sách",
        TrangThaiSach.NgungPhucVu => "Ngừng phục vụ",
        _ => value.ToString()
    };

    public static IEnumerable<SelectListItem> DanhSachEnum<TEnum>(TEnum? selected = null) where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>().Select(v => new SelectListItem
        {
            Value = v.ToString(),
            Text = Enum.GetName(typeof(TEnum), v) ?? v.ToString(),
            Selected = selected.HasValue && EqualityComparer<TEnum>.Default.Equals(selected.Value, v)
        });
    }
}
