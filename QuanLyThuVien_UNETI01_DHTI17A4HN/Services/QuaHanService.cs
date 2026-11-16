// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Tự động cập nhật trạng thái phiếu mượn đã quá hạn trong SQL Server.
// Bổ sung Module 5: Phan Vũ Thanh Thư - 23103100316
// Nội dung thực hiện: Truy vấn danh sách phiếu quá hạn cùng bạn đọc và chi tiết sách.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public class QuaHanService : IQuaHanService
{
    private readonly AppDbContext _db;

    public QuaHanService(AppDbContext db)
    {
        _db = db;
    }

    public async Task QuetQuaHanAsync()
    {
        var homNay = DateTime.Today;
        await _db.PhieuMuons
            .Where(p => p.TrangThai == TrangThaiPhieu.DangMuon && p.HanTra < homNay)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(p => p.TrangThai, TrangThaiPhieu.QuaHan));
    }

    public async Task<List<PhieuMuon>> LayDanhSachQuaHanAsync()
    {
        await QuetQuaHanAsync();
        return await _db.PhieuMuons.AsNoTracking()
            .Include(p => p.BanDoc)
            .Include(p => p.ChiTietMuons).ThenInclude(ct => ct.Sach)
            .Where(p => p.TrangThai == TrangThaiPhieu.QuaHan)
            .OrderBy(p => p.HanTra)
            .ThenBy(p => p.MaPhieuMuon)
            .ToListAsync();
    }
}
