using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public interface ISachService
{
    Task<SachIndexViewModel> LayDanhSachAsync(SachLocViewModel loc, List<int> sachTrongGio);
    Task<SachChiTietViewModel?> LayChiTietAsync(int id, bool choPhepQuanLy, List<int> sachTrongGio);
    Task<SachFormViewModel> TaoFormAsync(Sach sach);
    Task<Sach?> LayTheoMaAsync(int id);
    Task<ServiceResult> KiemTraAsync(Sach sach);
    Task<ServiceResult> TaoAsync(Sach sach);
    Task<ServiceResult> CapNhatAsync(int id, Sach sach);
    Task<ServiceResult> DoiTrangThaiAsync(int id);
    Task<ServiceResult> XoaAsync(int id);
    Task<bool> CoLichSuMuonAsync(int id);
}
