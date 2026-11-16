// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Truy vấn EF Core, lọc/sắp xếp và CRUD sách (Module 2).

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using SachEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Sach;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;

public class SachRepository : ISachRepository
{
    private readonly AppDbContext _context;

    public SachRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<SachEntity> TruyVanDanhSach(string? tuKhoa, int? maTheLoai, int? namXuatBan,
        TrangThaiSach? trangThai, bool? conSach, decimal? giaTu, decimal? giaDen, string sapXep)
    {
        var truyVan = _context.Sachs
            .AsNoTracking()
            .Include(s => s.TheLoai)
            .Include(s => s.NhaXuatBan)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var tuKhoaDaChuanHoa = tuKhoa.Trim();
            truyVan = truyVan.Where(s =>
                s.TenSach.Contains(tuKhoaDaChuanHoa)
                || (s.TacGia != null && s.TacGia.Contains(tuKhoaDaChuanHoa))
                || (s.TheLoai != null && s.TheLoai.TenTheLoai.Contains(tuKhoaDaChuanHoa))
                || (s.NhaXuatBan != null && s.NhaXuatBan.TenNhaXuatBan.Contains(tuKhoaDaChuanHoa)));
        }

        if (maTheLoai.HasValue) truyVan = truyVan.Where(s => s.MaTheLoai == maTheLoai.Value);
        if (namXuatBan.HasValue) truyVan = truyVan.Where(s => s.NamXuatBan == namXuatBan.Value);
        if (trangThai.HasValue) truyVan = truyVan.Where(s => s.TrangThai == trangThai.Value);
        if (conSach == true) truyVan = truyVan.Where(s => s.SoLuongCon > 0);
        if (conSach == false) truyVan = truyVan.Where(s => s.SoLuongCon <= 0);
        if (giaTu.HasValue) truyVan = truyVan.Where(s => s.DonGia >= giaTu.Value);
        if (giaDen.HasValue) truyVan = truyVan.Where(s => s.DonGia <= giaDen.Value);

        return sapXep switch
        {
            "ten_desc" => truyVan.OrderByDescending(s => s.TenSach).ThenBy(s => s.MaSach),
            "nam_asc" => truyVan.OrderBy(s => s.NamXuatBan).ThenBy(s => s.MaSach),
            "nam_desc" => truyVan.OrderByDescending(s => s.NamXuatBan).ThenBy(s => s.MaSach),
            "gia_asc" => truyVan.OrderBy(s => s.DonGia).ThenBy(s => s.MaSach),
            "gia_desc" => truyVan.OrderByDescending(s => s.DonGia).ThenBy(s => s.MaSach),
            "con_asc" => truyVan.OrderBy(s => s.SoLuongCon).ThenBy(s => s.MaSach),
            "con_desc" => truyVan.OrderByDescending(s => s.SoLuongCon).ThenBy(s => s.MaSach),
            _ => truyVan.OrderBy(s => s.TenSach).ThenBy(s => s.MaSach),
        };
    }

    public async Task<Models.Sach?> GetByIdAsync(int id)
    {
        return await _context.Sachs
            .Include(s => s.NhaXuatBan)
            .Include(s => s.TheLoai)
            .FirstOrDefaultAsync(s => s.MaSach == id);
    }

    public async Task<bool> KhoaNgoaiHopLeAsync(int maTheLoai, int maNhaXuatBan, bool choPhepNgungHoatDong = false)
    {
        var theLoaiQuery = _context.TheLoais.Where(t => t.MaTheLoai == maTheLoai);
        var nhaXuatBanQuery = _context.NhaXuatBans.Where(n => n.MaNhaXuatBan == maNhaXuatBan);
        if (!choPhepNgungHoatDong)
        {
            theLoaiQuery = theLoaiQuery.Where(t => t.TrangThai);
            nhaXuatBanQuery = nhaXuatBanQuery.Where(n => n.TrangThai);
        }

        var theLoaiHopLe = await theLoaiQuery.AnyAsync();
        var nhaXuatBanHopLe = await nhaXuatBanQuery.AnyAsync();
        return theLoaiHopLe && nhaXuatBanHopLe;
    }

    public Task<bool> IsInUseAsync(int id)
        => _context.ChiTietMuons.AnyAsync(ct => ct.MaSach == id);

    public Task AddAsync(Models.Sach sach)
    {
        _context.Sachs.Add(sach);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Models.Sach sach)
    {
        _context.Sachs.Update(sach);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Models.Sach sach)
    {
        _context.Sachs.Remove(sach);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}