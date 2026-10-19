// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Module 5 – Nghiệp vụ Chi tiết mượn: chỉ thêm/sửa/xóa khi phiếu "Chờ duyệt",
//                     không trùng sách, số lượng > 0, phiếu còn ít nhất 1 cuốn, kiểm tra lại điều kiện mượn.
 
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class ChiTietMuonService : IChiTietMuonService
{
    private readonly IChiTietMuonRepository _repo;
    private readonly IMuonSachService _muonSach;
 
    public ChiTietMuonService(IChiTietMuonRepository repo, IMuonSachService muonSach)
    {
        _repo = repo;
        _muonSach = muonSach;
    }
 
    public async Task<ChiTietMuonIndexViewModel?> LayDanhSachAsync(int maPhieu)
    {
        var phieu = await _repo.LayPhieuKemChiTietAsync(maPhieu, theoDoi: false);
        if (phieu is null) return null;
 
        var sachs = await _repo.LaySachCoTheThemAsync(phieu.ChiTietMuons.Select(ct => ct.MaSach));
        var vm = new ChiTietMuonIndexViewModel
        {
            Phieu = phieu,
            DsSachCoTheThem = sachs
                .Select(s => new SelectListItem($"{s.TenSach} (còn {s.SoLuongCon})", s.MaSach.ToString()))
                .ToList(),
        };
        if (phieu.TrangThai == TrangThaiPhieu.ChoDuyet)
        {
            vm.CanhBao = await _muonSach.KiemTraDieuKienAsync(phieu.MaBanDoc,
                phieu.ChiTietMuons.Select(ct => new SachYeuCau(ct.MaSach, ct.SoLuong)).ToList(), phieu.MaPhieuMuon);
        }
        return vm;
    }
 
    public async Task<ServiceResult> ThemAsync(int maPhieu, int maSach, int soLuong)
    {
        var phieu = await _repo.LayPhieuKemChiTietAsync(maPhieu, theoDoi: true);
        if (phieu is null) return ServiceResult.KhongTonTai();
 
        var loi = KiemTraPhieuChoDuyet(phieu);
        if (loi is null && soLuong <= 0) loi = "Số lượng phải lớn hơn 0.";
        if (loi is null && phieu.ChiTietMuons.Any(ct => ct.MaSach == maSach)) loi = "Sách đã có trong phiếu.";
        if (loi is null)
        {
            var dsMoi = phieu.ChiTietMuons.Select(ct => new SachYeuCau(ct.MaSach, ct.SoLuong))
                .Append(new SachYeuCau(maSach, soLuong)).ToList();
            var dsLoi = await _muonSach.KiemTraDieuKienAsync(phieu.MaBanDoc, dsMoi, maPhieu);
            if (dsLoi.Count > 0) return ServiceResult.TuDanhSach(dsLoi);
        }
        if (loi is not null) return ServiceResult.Loi(loi);
 
        phieu.ChiTietMuons.Add(new ChiTietMuon { MaSach = maSach, SoLuong = soLuong, TinhTrangMuon = TinhTrangMuon.BinhThuong });
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok("Đã thêm sách vào phiếu.");
    }
 
    public async Task<ServiceResult<ChiTietMuonFormViewModel>> LayFormSuaAsync(int id)
    {
        var ct = await _repo.LayKemPhieuAsync(id, theoDoi: false);
        if (ct is null) return ServiceResult<ChiTietMuonFormViewModel>.KhongTonTai();
 
        var kq = new ServiceResult<ChiTietMuonFormViewModel>
        {
            DuLieu = new ChiTietMuonFormViewModel
            {
                MaChiTiet = ct.MaChiTiet,
                MaPhieuMuon = ct.MaPhieuMuon,
                TenSach = ct.Sach?.TenSach ?? "",
                SoLuongCon = ct.Sach?.SoLuongCon ?? 0,
                SoLuong = ct.SoLuong,
                TinhTrangMuon = ct.TinhTrangMuon,
                GhiChu = ct.GhiChu,
            },
        };
        var loi = KiemTraPhieuChoDuyet(ct.PhieuMuon!);
        if (loi is not null) kq.ThemLoi(loi);
        return kq;
    }
 
    public async Task<ServiceResult> CapNhatAsync(int id, ChiTietMuonFormViewModel vm)
    {
        var ct = await _repo.LayKemPhieuAsync(id, theoDoi: true);
        if (ct is null) return ServiceResult.KhongTonTai();
 
        var loi = KiemTraPhieuChoDuyet(ct.PhieuMuon!);
        if (loi is not null) return ServiceResult.Loi(loi);
 
        var dsMoi = ct.PhieuMuon!.ChiTietMuons
            .Select(c => new SachYeuCau(c.MaSach, c.MaChiTiet == id ? vm.SoLuong : c.SoLuong)).ToList();
        var dsLoi = await _muonSach.KiemTraDieuKienAsync(ct.PhieuMuon.MaBanDoc, dsMoi, ct.MaPhieuMuon);
        if (dsLoi.Count > 0) return ServiceResult.TuDanhSach(dsLoi);
 
        ct.SoLuong = vm.SoLuong;
        ct.TinhTrangMuon = vm.TinhTrangMuon;
        ct.GhiChu = vm.GhiChu;
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok("Đã cập nhật chi tiết mượn.");
    }
 
    public async Task<ServiceResult<int>> XoaAsync(int id)
    {
        var ct = await _repo.LayKemPhieuAsync(id, theoDoi: true);
        if (ct is null) return ServiceResult<int>.KhongTonTai();
 
        var loi = KiemTraPhieuChoDuyet(ct.PhieuMuon!);
        if (loi is null && ct.PhieuMuon!.ChiTietMuons.Count <= 1)
            loi = "Phiếu phải còn ít nhất 1 cuốn sách. Nếu không muốn cho mượn, hãy từ chối phiếu.";
 
        var kq = new ServiceResult<int> { DuLieu = ct.MaPhieuMuon };
        if (loi is not null)
        {
            kq.ThemLoi(loi);
            return kq;
        }
 
        _repo.Remove(ct);
        await _repo.SaveChangesAsync();
        kq.ThongBao = "Đã bỏ sách khỏi phiếu.";
        return kq;
    }
 
    private static string? KiemTraPhieuChoDuyet(PhieuMuon phieu)
        => phieu.TrangThai == TrangThaiPhieu.ChoDuyet
            ? null
            : $"Phiếu #{phieu.MaPhieuMuon} đang ở trạng thái \"{phieu.TrangThai.TenHienThi()}\" – chỉ điều chỉnh chi tiết khi phiếu Chờ duyệt.";
}
