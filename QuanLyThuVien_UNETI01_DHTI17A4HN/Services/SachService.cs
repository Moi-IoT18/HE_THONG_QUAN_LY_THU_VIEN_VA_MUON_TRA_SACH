using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public class SachService : ISachService
{
    public Task<SachIndexViewModel> LayDanhSachAsync(SachLocViewModel loc, List<int> sachTrongGio)
        => Task.FromResult(new SachIndexViewModel { Loc = loc, SachTrongGio = new HashSet<int>(sachTrongGio) });

    public Task<SachChiTietViewModel?> LayChiTietAsync(int id, bool choPhepQuanLy, List<int> sachTrongGio)
        => Task.FromResult<SachChiTietViewModel?>(null);

    public Task<SachFormViewModel> TaoFormAsync(Sach sach)
        => Task.FromResult(new SachFormViewModel { Sach = sach });

    public Task<Sach?> LayTheoMaAsync(int id) => Task.FromResult<Sach?>(null);

    public Task<ServiceResult> KiemTraAsync(Sach sach) => Task.FromResult(ServiceResult.Ok("OK"));

    public Task<ServiceResult> TaoAsync(Sach sach) => Task.FromResult(ServiceResult.Ok("Đã thêm sách."));

    public Task<ServiceResult> CapNhatAsync(int id, Sach sach) => Task.FromResult(ServiceResult.Ok("Đã cập nhật sách."));

    public Task<ServiceResult> DoiTrangThaiAsync(int id) => Task.FromResult(ServiceResult.Ok("Đã đổi trạng thái."));

    public Task<ServiceResult> XoaAsync(int id) => Task.FromResult(ServiceResult.Ok("Đã xóa sách."));

    public Task<bool> CoLichSuMuonAsync(int id) => Task.FromResult(false);
}
