using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class SessionAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public string? Roles { get; set; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        var maTaiKhoan = session.LayMaTaiKhoan();
        if (!maTaiKhoan.HasValue)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var vaiTro = session.LayVaiTro();
        var db = context.HttpContext.RequestServices?.GetService<AppDbContext>();
        if (db is not null)
        {
            var taiKhoan = await db.TaiKhoans.AsNoTracking()
                .Include(t => t.BanDoc)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan.Value);

            if (taiKhoan is null || !taiKhoan.TrangThai)
            {
                session.Clear();
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            vaiTro = Quyen.TuVaiTro(taiKhoan.VaiTro);
            session.SetString(SessionKeys.VaiTro, vaiTro);
            session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
            if (taiKhoan.BanDoc is not null)
            {
                session.SetInt32(SessionKeys.MaBanDoc, taiKhoan.BanDoc.MaBanDoc);
            }
        }

        if (Roles is not null && (vaiTro is null || !Roles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Contains(vaiTro, StringComparer.Ordinal)))
        {
            context.Result = new RedirectToActionResult("TuChoiTruyCap", "Auth", null);
        }
    }
}
