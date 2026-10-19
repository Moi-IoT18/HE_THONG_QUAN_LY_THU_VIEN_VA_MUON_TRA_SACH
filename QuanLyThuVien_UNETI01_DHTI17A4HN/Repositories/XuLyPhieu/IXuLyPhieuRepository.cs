// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Module 4 – Interface truy c?p d? li?u phi?u mu?n phía Th? thu (x? lý phi?u).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

public interface IXuLyPhieuRepository : IRepository<PhieuMuon>
{
    IQueryable<PhieuMuon> TruyVanDanhSach(PhieuMuonLocViewModel loc);
    Task<Dictionary<TrangThaiPhieu, int>> DemTheoTrangThaiAsync();
    Task<PhieuMuon?> LayDayDuAsync(int maPhieu, bool theoDoi);
    Task<List<TheLoai>> LayTheLoaiAsync();
}
