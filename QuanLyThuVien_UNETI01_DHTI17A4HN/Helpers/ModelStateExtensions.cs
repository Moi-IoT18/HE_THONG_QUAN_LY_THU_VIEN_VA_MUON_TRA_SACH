using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class ModelStateExtensions
{
    public static void ThemLoiService(this ModelStateDictionary modelState, ServiceResult result, string prefix = "")
    {
        if (result is null || result.ThanhCong)
            return;

        var key = string.IsNullOrWhiteSpace(prefix) ? result.FieldName ?? string.Empty : prefix + (result.FieldName ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(key) && !modelState.ContainsKey(key))
        {
            modelState.AddModelError(key, result.ThongBao);
            return;
        }

        modelState.AddModelError(string.Empty, result.ThongBao);
    }
}
