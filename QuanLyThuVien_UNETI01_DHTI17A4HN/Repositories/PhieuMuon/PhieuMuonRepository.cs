// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Truy vấn LINQ / EF Core cho Phiếu mượn phía Bạn đọc.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public class PhieuMuonRepository : Repository<PhieuMuon>, IPhieuMuonRepository
{
    public PhieuMuonRepository(AppDbContext db) : base(db) { }
 
    public IQueryable<PhieuMuon> TruyVanCuaBanDoc(int maBanDoc, TrangThaiPhieu? trangThai)
    {
        var q = _dbSet.AsNoTracking()
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach)
            .Where(p => p.MaBanDoc == maBanDoc);
        if (trangThai.HasValue)
        {
            q = q.Where(p => p.TrangThai == trangThai.Value);
        }
        return q.OrderByDescending(p => p.NgayMuon);
    }
 
    public Task<List<PhieuMuon>> LayCanhBaoHanAsync(int maBanDoc, DateTime nguongSapHan)
        => _dbSet.AsNoTracking()
            .Where(p => p.MaBanDoc == maBanDoc
                        && (p.TrangThai == TrangThaiPhieu.QuaHan
                            || (p.TrangThai == TrangThaiPhieu.DangMuon && p.HanTra <= nguongSapHan)))
            .OrderBy(p => p.HanTra)
            .ToListAsync();
 
    public Task<PhieuMuon?> LayChiTietCuaBanDocAsync(int maPhieu, int maBanDoc)
        => _dbSet.AsNoTracking()
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach)
            .Include(p => p.NhanVien)
            .FirstOrDefaultAsync(p => p.MaPhieuMuon == maPhieu && p.MaBanDoc == maBanDoc);
 
    public Task<PhieuMuon?> TimCuaBanDocAsync(int maPhieu, int maBanDoc)
        => _dbSet.FirstOrDefaultAsync(p => p.MaPhieuMuon == maPhieu && p.MaBanDoc == maBanDoc);
 
    public Task<bool> CoPhieuQuaHanAsync(int maBanDoc, int? maPhieuBoQua)
        => _dbSet.AnyAsync(p =>
            p.MaBanDoc == maBanDoc
            && p.MaPhieuMuon != maPhieuBoQua
            && (p.TrangThai == TrangThaiPhieu.QuaHan
                || (p.TrangThai == TrangThaiPhieu.DangMuon && p.HanTra < DateTime.Today)));
 
    public IQueryable<PhieuMuon> TruyVanPhieuDangGiu(int maBanDoc, int? maPhieuBoQua)
        => _dbSet.Where(p =>
            p.MaBanDoc == maBanDoc
            && p.MaPhieuMuon != maPhieuBoQua
            && (p.TrangThai == TrangThaiPhieu.ChoDuyet
                || p.TrangThai == TrangThaiPhieu.DangMuon
                || p.TrangThai == TrangThaiPhieu.QuaHan));
}
