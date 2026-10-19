// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Truy vấn LINQ / EF Core cho bảng ChiTietMuon.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public class ChiTietMuonRepository : Repository<ChiTietMuon>, IChiTietMuonRepository
{
    public ChiTietMuonRepository(AppDbContext db) : base(db) { }
 
    public Task<PhieuMuon?> LayPhieuKemChiTietAsync(int maPhieu, bool theoDoi)
    {
        IQueryable<PhieuMuon> q = _db.PhieuMuons
            .Include(p => p.BanDoc)
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach);
        if (!theoDoi) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(p => p.MaPhieuMuon == maPhieu);
    }
 
    public Task<List<Sach>> LaySachCoTheThemAsync(IEnumerable<int> maSachDaCo)
    {
        var daCo = maSachDaCo.ToList();
        return _db.Sachs.AsNoTracking()
            .Where(s => s.TrangThai == TrangThaiSach.ConPhucVu && s.SoLuongCon > 0 && !daCo.Contains(s.MaSach))
            .OrderBy(s => s.TenSach)
            .ToListAsync();
    }
 
    public Task<ChiTietMuon?> LayKemPhieuAsync(int maChiTiet, bool theoDoi)
    {
        IQueryable<ChiTietMuon> q = _dbSet
            .Include(c => c.Sach)
            .Include(c => c.PhieuMuon).ThenInclude(p => p!.ChiTietMuons);
        if (!theoDoi) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(c => c.MaChiTiet == maChiTiet);
    }
}
