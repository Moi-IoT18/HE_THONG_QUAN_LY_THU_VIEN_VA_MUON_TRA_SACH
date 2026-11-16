// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Truy vấn LINQ / EF Core cho Phiếu mượn phía Bạn đọc.
// Bổ sung Module 4: Trịnh Thị Thanh Tâm - 23103100193
// Nội dung thực hiện: Tìm kiếm/lọc/sắp xếp phiếu xử lý và tải đủ dữ liệu liên quan.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
public class PhieuMuonRepository : Repository<PhieuMuon>, IPhieuMuonRepository
{
    public PhieuMuonRepository(AppDbContext db) : base(db) { }

    public IQueryable<PhieuMuon> TruyVanXuLy(string? tuKhoa, int? maTheLoai, int? maNhaXuatBan,
        TrangThaiPhieu? trangThai, DateTime? tuNgay, DateTime? denNgay, string sapXep)
    {
        var q = _dbSet.AsNoTracking()
            .Include(p => p.BanDoc).ThenInclude(b => b!.TaiKhoan)
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach).ThenInclude(s => s!.TheLoai)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var tuKhoaDaChuanHoa = tuKhoa.Trim();
            q = q.Where(p =>
                p.BanDoc!.HoTen.Contains(tuKhoaDaChuanHoa)
                || p.BanDoc.TaiKhoan!.TenDangNhap.Contains(tuKhoaDaChuanHoa)
                || p.ChiTietMuons.Any(ct => ct.Sach!.TenSach.Contains(tuKhoaDaChuanHoa)));
        }

        if (maTheLoai.HasValue)
            q = q.Where(p => p.ChiTietMuons.Any(ct => ct.Sach!.MaTheLoai == maTheLoai.Value));
        if (maNhaXuatBan.HasValue)
            q = q.Where(p => p.ChiTietMuons.Any(ct => ct.Sach!.MaNhaXuatBan == maNhaXuatBan.Value));
        if (trangThai.HasValue)
            q = q.Where(p => p.TrangThai == trangThai.Value);
        if (tuNgay.HasValue)
            q = q.Where(p => p.NgayMuon >= tuNgay.Value.Date);
        if (denNgay.HasValue)
        {
            var ngayCuoi = denNgay.Value.Date;
            if (ngayCuoi < DateTime.MaxValue.Date)
            {
                var denNgayKhongBaoGom = ngayCuoi.AddDays(1);
                q = q.Where(p => p.NgayMuon < denNgayKhongBaoGom);
            }
        }

        return sapXep switch
        {
            "ngay_asc" => q.OrderBy(p => p.NgayMuon).ThenBy(p => p.MaPhieuMuon),
            "ten_asc" => q.OrderBy(p => p.BanDoc!.HoTen).ThenByDescending(p => p.NgayMuon),
            "ten_desc" => q.OrderByDescending(p => p.BanDoc!.HoTen).ThenByDescending(p => p.NgayMuon),
            _ => q.OrderByDescending(p => p.NgayMuon).ThenByDescending(p => p.MaPhieuMuon),
        };
    }

    public Task<PhieuMuon?> LayChiTietXuLyAsync(int maPhieu, bool theoDoi = false)
    {
        var q = theoDoi ? _dbSet.AsQueryable() : _dbSet.AsNoTracking();
        return q
            .Include(p => p.BanDoc).ThenInclude(b => b!.TaiKhoan)
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach).ThenInclude(s => s!.TheLoai)
            .Include(p => p.NhanVien)
            .Include(p => p.NhanVienThuTienPhat)
            .FirstOrDefaultAsync(p => p.MaPhieuMuon == maPhieu);
    }

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
            .Include(p => p.NhanVienThuTienPhat)
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
                || p.TrangThai == TrangThaiPhieu.DaDuyet
                || p.TrangThai == TrangThaiPhieu.DangMuon
                || p.TrangThai == TrangThaiPhieu.QuaHan));
}
