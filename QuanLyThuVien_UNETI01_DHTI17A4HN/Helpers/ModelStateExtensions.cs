using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class ModelStateExtensions
{
    public static void ThemLoiService(this ModelStateDictionary modelState, ServiceResult result)
    {
        if (result.KhongTimThay)
        {
            modelState.AddModelError(string.Empty, "Dữ liệu không tồn tại.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(result.LoiGop))
            modelState.AddModelError(string.Empty, result.LoiGop);

        foreach (var (fieldName, error) in result.LoiTheoTruong)
        {
            modelState.AddModelError(fieldName, error);
        }
    }
}
