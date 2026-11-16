using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using TheLoaiEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TheLoai;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TheLoai;

public class TheLoaiRepository : ITheLoaiRepository
{
    private readonly AppDbContext _context;

    public TheLoaiRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TheLoaiEntity>> GetAllAsync()
    {
        return await _context.TheLoais
            .OrderBy(t => t.ThuTuHienThi)
            .ThenBy(t => t.TenTheLoai)
            .ToListAsync();
    }

    public async Task<TheLoaiEntity?> GetByIdAsync(int id)
    {
        return await _context.TheLoais.FirstOrDefaultAsync(t => t.MaTheLoai == id);
    }

    public async Task<bool> ExistsAsync(string tenTheLoai, int? excludeId = null)
    {
        var normalized = tenTheLoai.Trim();
        return await _context.TheLoais.AnyAsync(t =>
            t.TenTheLoai == normalized && (!excludeId.HasValue || t.MaTheLoai != excludeId.Value));
    }

    public async Task<bool> IsInUseAsync(int id)
    {
        return await _context.Sachs.AnyAsync(s => s.MaTheLoai == id);
    }

    public Task AddAsync(TheLoaiEntity entity)
    {
        _context.TheLoais.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TheLoaiEntity entity)
    {
        _context.TheLoais.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TheLoaiEntity entity)
    {
        _context.TheLoais.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
