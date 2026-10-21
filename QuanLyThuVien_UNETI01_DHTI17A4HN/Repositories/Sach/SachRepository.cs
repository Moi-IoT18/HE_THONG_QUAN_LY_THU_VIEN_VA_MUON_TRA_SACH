// Ho va ten: Tran Gia Nghia
// Ma sinh vien: 23103100228
// Noi dung thuc hien: Trien khai Repository xu ly truy van du lieu Sach (Module 2 - SV2).

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;

public class SachRepository
{
    private readonly AppDbContext _context;

    public SachRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Sach>> GetAllAsync()
    {
        return await _context.Sachs
            .Include(s => s.NhaXuatBan)
            .Include(s => s.TheLoai)
            .ToListAsync();
    }

    public async Task<Models.Sach?> GetByIdAsync(int id)
    {
        return await _context.Sachs
            .Include(s => s.NhaXuatBan)
            .Include(s => s.TheLoai)
            .FirstOrDefaultAsync(s => s.MaSach == id);
    }

    public async Task AddAsync(Models.Sach sach)
    {
        await _context.Sachs.AddAsync(sach);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Sach sach)
    {
        _context.Sachs.Update(sach);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sach = await _context.Sachs.FindAsync(id);
        if (sach != null)
        {
            _context.Sachs.Remove(sach);
            await _context.SaveChangesAsync();
        }
    }
}