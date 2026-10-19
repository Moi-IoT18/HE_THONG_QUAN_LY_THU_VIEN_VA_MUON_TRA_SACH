using Microsoft.AspNetCore.Http;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class SessionExtensions
{
    private const string VaiTroKey = "VaiTro";
    private const string DaDangNhapKey = "DaDangNhap";

    public static VaiTro LayVaiTro(this ISession session)
    {
        var value = session.GetString(VaiTroKey);
        return Enum.TryParse<VaiTro>(value, out var vaiTro) ? vaiTro : VaiTro.BanDoc;
    }

    public static bool LaNhanVien(this ISession session)
    {
        var vaiTro = session.LayVaiTro();
        return vaiTro == VaiTro.ThuThu || vaiTro == VaiTro.Admin;
    }

    public static bool DaDangNhap(this ISession session)
    {
        return session.GetString(DaDangNhapKey) == "1" || session.LayVaiTro() != VaiTro.BanDoc;
    }
}
