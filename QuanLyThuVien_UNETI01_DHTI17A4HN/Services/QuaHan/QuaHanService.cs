using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public sealed class QuaHanService : IQuaHanService
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
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.TrangThai, TrangThaiPhieu.QuaHan));
    }

    public static int TinhSoNgayQuaHan(DateTime hanTra, DateTime? ngayTinh = null)
    {
        var mocTinh = (ngayTinh ?? DateTime.Today).Date;
        return Math.Max(0, (mocTinh - hanTra.Date).Days);
    }
}
