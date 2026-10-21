// H? và tên: Nguy?n Th? Ðông
// Mã sinh viên: 23103100187
// N?i dung th?c hi?n: C?u hình DI, DbContext, Session và Pipeline cho ?ng d?ng ASP.NET Core MVC.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// C?u hình k?t n?i SQL Server v?i AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ðang ký Repository và Service (Dependency Injection)
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// C?u hình Session ph?c v? dang nh?p
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// S? d?ng Session Middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run()
