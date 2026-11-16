// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Kiểm thử quyền Session ở Controller cho khách, sai vai trò và đúng vai trò.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

namespace QuanLyThuVien.Tests;

public sealed class AuthorizationTests
{
    [Fact]
    public async Task Authorization_RedirectsAnonymousToLogin()
    {
        var context = TaoContext(new TestSession());
        var filter = new SessionAuthorizeAttribute { Roles = Quyen.NhanVienVaQuanLy };

        await filter.OnAuthorizationAsync(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Auth", redirect.ControllerName);
    }

    [Fact]
    public async Task Authorization_DeniesReaderFromStaffArea()
    {
        var session = new TestSession();
        session.SetInt32(SessionKeys.MaTaiKhoan, 42);
        session.SetString(SessionKeys.VaiTro, Quyen.BanDoc);
        var context = TaoContext(session);
        var filter = new SessionAuthorizeAttribute { Roles = Quyen.NhanVienVaQuanLy };

        await filter.OnAuthorizationAsync(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("TuChoiTruyCap", redirect.ActionName);
    }

    [Fact]
    public async Task Authorization_AllowsStaffRole()
    {
        var session = new TestSession();
        session.SetInt32(SessionKeys.MaTaiKhoan, 42);
        session.SetString(SessionKeys.VaiTro, Quyen.NhanVien);
        var context = TaoContext(session);
        var filter = new SessionAuthorizeAttribute { Roles = Quyen.NhanVienVaQuanLy };

        await filter.OnAuthorizationAsync(context);

        Assert.Null(context.Result);
        Assert.Equal(42, session.LayMaTaiKhoan());
    }

    [Fact]
    public void SessionExtensions_ReadsAccountAndReaderIdsAsIntegers()
    {
        var session = new TestSession();
        session.SetInt32(SessionKeys.MaTaiKhoan, 42);
        session.SetInt32(SessionKeys.MaBanDoc, 17);

        Assert.Equal(42, session.LayMaTaiKhoan());
        Assert.Equal(17, session.LayMaBanDoc());
    }

    private static AuthorizationFilterContext TaoContext(ISession session)
    {
        var httpContext = new DefaultHttpContext { Session = session };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _values = new(StringComparer.Ordinal);

        public bool IsAvailable => true;
        public string Id => "test-session";
        public IEnumerable<string> Keys => _values.Keys;
        public void Clear() => _values.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _values.Remove(key);
        public void Set(string key, byte[] value) => _values[key] = value;
        public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value)
            => _values.TryGetValue(key, out value);
    }
}
