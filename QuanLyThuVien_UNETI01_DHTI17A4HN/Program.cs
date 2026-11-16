// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Đăng ký DI cho repository/service sách, dữ liệu dùng chung (Module 2)
//                     và các luồng Bạn đọc / Phiếu mượn (Module 3).
// Bổ sung Module 3 bởi: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Đăng ký DI Module 3 và provisioning Admin ban đầu an toàn qua CLI.
// Bổ sung Module 5: Phan Vũ Thanh Thư - 23103100316
// Nội dung thực hiện: Đăng ký repository chi tiết mượn, Dashboard và thống kê.
//Nguyễn Thế Đông - 23103100187
// Nội dung thực hiện: Đăng ký repository và service nhà xuất bản, tài khoản, thể loại , hoàn chỉnh các module.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TheLoai;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.ChiTietMuon;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TheLoai;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Dashboard;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;
using QuaHanServiceImplementation = QuanLyThuVien_UNETI01_DHTI17A4HN.Services.QuaHanService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ISachService, SachService>();
builder.Services.AddScoped<ISachRepository, SachRepository>();
builder.Services.AddScoped<IBookImageStorage, BookImageStorage>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailNotificationService, SmtpEmailNotificationService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INhaXuatBanRepository, NhaXuatBanRepository>();
builder.Services.AddScoped<INhaXuatBanService, NhaXuatBanService>();
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITheLoaiRepository, TheLoaiRepository>();
builder.Services.AddScoped<ITheLoaiService, TheLoaiService>();
builder.Services.AddScoped<IBanDocRepository, BanDocRepository>();
builder.Services.AddScoped<IPhieuMuonRepository, PhieuMuonRepository>();
builder.Services.AddScoped<IBanDocService, BanDocService>();
builder.Services.AddScoped<IPhieuMuonService, PhieuMuonService>();
builder.Services.AddScoped<IMuonSachService, MuonSachService>();
builder.Services.AddScoped<IQuaHanService, QuaHanServiceImplementation>();
builder.Services.AddScoped<IChiTietMuonRepository, ChiTietMuonRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IThongKeExportService, ThongKeExportService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

if (args.Contains("--provision-admin", StringComparer.Ordinal))
{
    var tenDangNhap = Environment.GetEnvironmentVariable("LIBRARY_INITIAL_ADMIN_USERNAME")?.Trim();
    var matKhau = Environment.GetEnvironmentVariable("LIBRARY_INITIAL_ADMIN_PASSWORD");
    if (string.IsNullOrWhiteSpace(tenDangNhap) || tenDangNhap.Length is < 3 or > 50
        || string.IsNullOrWhiteSpace(matKhau) || matKhau.Length is < 12 or > 128)
    {
        throw new InvalidOperationException(
            "Set LIBRARY_INITIAL_ADMIN_USERNAME (3-50 chars) and LIBRARY_INITIAL_ADMIN_PASSWORD (12-128 chars) before provisioning.");
    }

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Database.CanConnectAsync())
    {
        throw new InvalidOperationException("Cannot connect to the configured SQL Server database.");
    }
    if (await db.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap))
    {
        throw new InvalidOperationException("The requested username already exists.");
    }
    if (await db.TaiKhoans.AnyAsync(t => t.VaiTro == VaiTro.QuanLy))
    {
        throw new InvalidOperationException("An administrator already exists; initial provisioning is disabled.");
    }

    db.TaiKhoans.Add(new TaiKhoan
    {
        TenDangNhap = tenDangNhap,
        MatKhau = MatKhauHelper.BamMatKhau(matKhau),
        HoTen = "Quản trị hệ thống",
        VaiTro = VaiTro.QuanLy,
        TrangThai = true,
        NgayTao = DateTime.Now,
    });
    await db.SaveChangesAsync();
    Console.WriteLine("Initial administrator provisioned successfully.");
    return;
}

if (args.Contains("--seed-demo-data", StringComparer.Ordinal))
{
    var matKhauAdmin = Environment.GetEnvironmentVariable("LIBRARY_DEMO_ADMIN_PASSWORD");
    var matKhauThuThu = Environment.GetEnvironmentVariable("LIBRARY_DEMO_STAFF_PASSWORD");
    var matKhauBanDoc = Environment.GetEnvironmentVariable("LIBRARY_DEMO_READER_PASSWORD");
    if (new[] { matKhauAdmin, matKhauThuThu, matKhauBanDoc }
        .Any(password => string.IsNullOrWhiteSpace(password) || password.Length is < 8 or > 128))
    {
        throw new InvalidOperationException(
            "Set LIBRARY_DEMO_ADMIN_PASSWORD, LIBRARY_DEMO_STAFF_PASSWORD, and LIBRARY_DEMO_READER_PASSWORD (8-128 chars each); values are never printed or stored in plaintext.");
    }

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Database.CanConnectAsync())
    {
        throw new InvalidOperationException("Cannot connect to the configured SQL Server database.");
    }
    await DbSeeder.SeedDemoAsync(db, matKhauAdmin!, matKhauThuThu!, matKhauBanDoc!);
    Console.WriteLine("Demo data seeded. Each role uses its configured password; no password was printed.");
    return;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
