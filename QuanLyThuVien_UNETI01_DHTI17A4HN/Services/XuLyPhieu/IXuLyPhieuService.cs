// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Module 4 – Interface nghi?p v? x? lý phi?u: duy?t, t? ch?i, nh?n tr? + tính ph?t.

using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public interface IXuLyPhieuService
{
    Task<XuLyPhieuIndexViewModel> LayDanhSachAsync(PhieuMuonLocViewModel loc);
    Task<(PhieuMuon? Phieu, List<string>? CanhBao)> LayChiTietAsync(int id);
    Task<ServiceResult> DuyetAsync(int id, int? maNhanVien);
    Task<ServiceResult> TuChoiAsync(int id, string? lyDo, int? maNhanVien);
    Task<ServiceResult<TraSachViewModel>> LayFormTraSachAsync(int id);
    Task<ServiceResult<TraSachViewModel>> TraSachAsync(int id, TraSachViewModel vm, int? maNhanVien, string? tenNhanVien);
}
