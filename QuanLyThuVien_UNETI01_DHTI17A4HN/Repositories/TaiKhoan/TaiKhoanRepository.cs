using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using TaiKhoanModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;

public class TaiKhoanRepository : ITaiKhoanRepository
{
    private readonly AppDbContext _context;

    public TaiKhoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<TaiKhoanModel?> GetByTenDangNhapAsync(string tenDangNhap)
    {
        return _context.TaiKhoans
            .Include(t => t.BanDoc)
            .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);
    }

    public Task<List<TaiKhoanModel>> GetAllAsync()
    {
        return _context.TaiKhoans
            .OrderBy(t => t.TenDangNhap)
            .ToListAsync();
    }

    public Task<TaiKhoanModel?> GetByIdAsync(int id)
    {
        return _context.TaiKhoans
            .Include(t => t.BanDoc)
            .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
    }

    public Task<bool> ExistsAsync(string tenDangNhap, int? excludeId = null)
    {
        var normalized = tenDangNhap.Trim();
        return _context.TaiKhoans.AnyAsync(t =>
            t.TenDangNhap == normalized && (!excludeId.HasValue || t.MaTaiKhoan != excludeId.Value));
    }

    public Task AddAsync(TaiKhoanModel taiKhoan)
    {
        _context.TaiKhoans.Add(taiKhoan);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TaiKhoanModel taiKhoan)
    {
        _context.TaiKhoans.Update(taiKhoan);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
