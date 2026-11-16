// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Tra cứu chi tiết mượn, chỉ đọc kết quả nhân viên xác nhận khi nhận trả.

using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.ChiTietMuon;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
using ChiTietMuonModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.ChiTietMuon;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Controllers.ChiTietMuon;

[SessionAuthorize(Roles = Quyen.NhanVienVaQuanLy)]
public sealed class ChiTietMuonController : Controller
{
    private readonly IChiTietMuonRepository _repository;

    public ChiTietMuonController(IChiTietMuonRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ChiTietMuonViewModel model)
    {
        model.TuKhoa = model.TuKhoa?.Trim();
        if (model.TrangThai.HasValue && !Enum.IsDefined(model.TrangThai.Value))
            model.TrangThai = null;

        model.DanhSach = await PhanTrang<ChiTietMuonModel>.TaoAsync(
            _repository.TruyVanDanhSach(model.TuKhoa, model.TrangThai), model.Trang, QuyDinh.KichThuocTrang);
        model.Trang = model.DanhSach.TrangHienTai;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return BadRequest();
        var chiTiet = await _repository.LayChiTietAsync(id);
        return chiTiet is null ? NotFound() : View(chiTiet);
    }
}
