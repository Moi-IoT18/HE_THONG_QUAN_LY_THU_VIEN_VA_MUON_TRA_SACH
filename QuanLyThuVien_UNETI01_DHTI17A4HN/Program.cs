using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBanDocRepository, BanDocRepository>();
builder.Services.AddScoped<IPhieuMuonRepository, PhieuMuonRepository>();
builder.Services.AddScoped<IXuLyPhieuRepository, XuLyPhieuRepository>();
builder.Services.AddScoped<IBanDocService, BanDocService>();
builder.Services.AddScoped<IMuonSachService, MuonSachService>();
builder.Services.AddScoped<IPhieuMuonService, PhieuMuonService>();
builder.Services.AddScoped<IXuLyPhieuService, XuLyPhieuService>();
builder.Services.AddScoped<IQuaHanService, QuaHanService>();
builder.Services.AddScoped<ISachService, SachService>();
builder.Services.AddScoped<INhaXuatBanRepository, NhaXuatBanRepository>();
builder.Services.AddScoped<INhaXuatBanService, NhaXuatBanService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

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
    pattern: "{controller=Sach}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
