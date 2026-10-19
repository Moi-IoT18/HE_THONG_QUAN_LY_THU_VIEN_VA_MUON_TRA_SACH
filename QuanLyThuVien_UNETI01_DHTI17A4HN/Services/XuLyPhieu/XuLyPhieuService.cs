// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Module 4 – Nghi?p v? x? lý phi?u mu?n (Th? thu/Admin)

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public class XuLyPhieuService : IXuLyPhieuService
{
    private readonly IXuLyPhieuRepository _repo;
    private readonly IMuonSachService _muonSach;
    private readonly IQuaHanService _quaHan;

    public XuLyPhieuService(IXuLyPhieuRepository repo, IMuonSachService muonSach, IQuaHanService quaHan)
    {
        _repo = repo;
        _muonSach = muonSach;
        _quaHan = quaHan;
    }

    public async Task<XuLyPhieuIndexViewModel> LayDanhSachAsync(PhieuMuonLocViewModel loc)
    {
        await _quaHan.QuetQuaHanAsync();
        var theLoais = await _repo.LayTheLoaiAsync();
        return new XuLyPhieuIndexViewModel
        {
            Loc = loc,
            DanhSach = await PhanTrang<PhieuMuon>.TaoAsync(_repo.TruyVanDanhSach(loc), loc.Trang, QuyDinh.KichThuocTrang),
            DsTheLoai = theLoais.Select(t => new SelectListItem(t.TenTheLoai, t.MaTheLoai.ToString(), t.MaTheLoai == loc.MaTheLoai)).ToList(),
            DemTheoTrangThai = await _repo.DemTheoTrangThaiAsync(),
        };
    }

    public async Task<(PhieuMuon? Phieu, List<string>? CanhBao)> LayChiTietAsync(int id)
    {
        var phieu = await _repo.LayDayDuAsync(id, theoDoi: false);
        if (phieu is null) return (null, null);

        List<string>? canhBao = null;
        if (phieu.TrangThai == TrangThaiPhieu.ChoDuyet)
        {
            canhBao = await _muonSach.KiemTraDieuKienAsync(phieu.MaBanDoc, YeuCauCua(phieu), phieu.MaPhieuMuon);
        }
        return (phieu, canhBao);
    }

    public async Task<ServiceResult> DuyetAsync(int id, int? maNhanVien)
    {
        var phieu = await _repo.LayDayDuAsync(id, theoDoi: true);
        if (phieu is null) return ServiceResult.KhongTonTai();

        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DangMuon))
            return ServiceResult.Loi(TrangThaiPhieuService.ThongBaoKhongHopLe(phieu.TrangThai, TrangThaiPhieu.DangMuon));

        var loi = await _muonSach.KiemTraDieuKienAsync(phieu.MaBanDoc, YeuCauCua(phieu), phieu.MaPhieuMuon);
        if (loi.Count > 0) return ServiceResult.Loi("Không th? duy?t phi?u:\n• " + string.Join("\n• ", loi));

        foreach (var ct in phieu.ChiTietMuons) { ct.Sach!.SoLuongCon -= ct.SoLuong; }

        var bayGio = DateTime.Now;
        phieu.TrangThai = TrangThaiPhieu.DangMuon;
        phieu.NhanVienXuLy = maNhanVien;
        phieu.NgayXuLy = bayGio;
        phieu.HanTra = bayGio.Date.AddDays(QuyDinh.SoNgayMuon);
        await _repo.SaveChangesAsync();

        return ServiceResult.Ok($"Ðã duy?t phi?u #{id}. H?n tr?: {phieu.HanTra:dd/MM/yyyy}.");
    }

    public async Task<ServiceResult> TuChoiAsync(int id, string? lyDo, int? maNhanVien)
    {
        var phieu = await _repo.GetByIdAsync(id);
        if (phieu is null) return ServiceResult.KhongTonTai();

        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.TuChoi))
            return ServiceResult.Loi(TrangThaiPhieuService.ThongBaoKhongHopLe(phieu.TrangThai, TrangThaiPhieu.TuChoi));
        if (string.IsNullOrWhiteSpace(lyDo)) return ServiceResult.Loi("Vui lòng nh?p lý do t? ch?i.");

        phieu.TrangThai = TrangThaiPhieu.TuChoi;
        phieu.GhiChu = "T? ch?i: " + lyDo.Trim()[..Math.Min(lyDo.Trim().Length, 400)];
        phieu.NhanVienXuLy = maNhanVien;
        phieu.NgayXuLy = DateTime.Now;
        await _repo.SaveChangesAsync();

        return ServiceResult.Ok($"Ðã t? ch?i phi?u #{id}.");
    }

    public async Task<ServiceResult<TraSachViewModel>> LayFormTraSachAsync(int id)
    {
        var phieu = await _repo.LayDayDuAsync(id, theoDoi: false);
        if (phieu is null) return ServiceResult<TraSachViewModel>.KhongTonTai();

        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DaTra))
            return ServiceResult<TraSachViewModel>.Loi(TrangThaiPhieuService.ThongBaoKhongHopLe(phieu.TrangThai, TrangThaiPhieu.DaTra));

        return ServiceResult<TraSachViewModel>.Ok(TaoFormTraSach(phieu));
    }

    public async Task<ServiceResult<TraSachViewModel>> TraSachAsync(int id, TraSachViewModel vm, int? maNhanVien, string? tenNhanVien)
    {
        var phieu = await _repo.LayDayDuAsync(id, theoDoi: true);
        if (phieu is null) return ServiceResult<TraSachViewModel>.KhongTonTai();

        if (!TrangThaiPhieuService.CoTheChuyen(phieu.TrangThai, TrangThaiPhieu.DaTra))
            return ServiceResult<TraSachViewModel>.Loi(TrangThaiPhieuService.ThongBaoKhongHopLe(phieu.TrangThai, TrangThaiPhieu.DaTra));

        var tinhTrang = vm.Dong.GroupBy(d => d.MaChiTiet).ToDictionary(g => g.Key, g => g.First());
        if (phieu.ChiTietMuons.Any(ct => !tinhTrang.ContainsKey(ct.MaChiTiet)) || vm.Dong.Any(d => !Enum.IsDefined(d.TinhTrangTra)))
        {
            var form = TaoFormTraSach(phieu);
            form.GhiChu = vm.GhiChu;
            var loi = new ServiceResult<TraSachViewModel> { DuLieu = form };
            loi.ThemLoi("Vui lòng ch?n tình tr?ng tr? cho t?t c? các sách.");
            return loi;
        }

        var bayGio = DateTime.Now;
        var soNgayQuaHan = QuaHanService.TinhSoNgayQuaHan(phieu.HanTra, bayGio);

        foreach (var ct in phieu.ChiTietMuons)
        {
            var dong = tinhTrang[ct.MaChiTiet];
            ct.TinhTrangTra = dong.TinhTrangTra;
            ct.SoNgayQuaHan = soNgayQuaHan;
            ct.TienPhat = TinhPhatService.TinhTienPhat(soNgayQuaHan, ct.SoLuong, ct.Sach!.DonGia, dong.TinhTrangTra);
            ct.GhiChu = string.IsNullOrWhiteSpace(dong.GhiChu) ? ct.GhiChu : dong.GhiChu.Trim();

            if (dong.TinhTrangTra == TinhTrangTra.Mat) ct.Sach.SoLuong -= ct.SoLuong;
            else ct.Sach.SoLuongCon += ct.SoLuong;
        }

        phieu.NgayTra = bayGio;
        phieu.TienPhat = phieu.ChiTietMuons.Sum(ct => ct.TienPhat);
        phieu.TrangThai = TrangThaiPhieu.DaTra;
        var ghiChuTra = $"Nh?n tr? b?i {tenNhanVien} lúc {bayGio:dd/MM/yyyy HH:mm}" + (string.IsNullOrWhiteSpace(vm.GhiChu) ? "" : $". {vm.GhiChu.Trim()}");
        phieu.GhiChu = string.IsNullOrWhiteSpace(phieu.GhiChu) ? ghiChuTra : $"{phieu.GhiChu} | {ghiChuTra}";
        if (phieu.GhiChu.Length > 500) phieu.GhiChu = phieu.GhiChu[..500];
        phieu.NhanVienXuLy ??= maNhanVien;
        await _repo.SaveChangesAsync();

        return new ServiceResult<TraSachViewModel>
        {
            ThongBao = phieu.TienPhat > 0 ? $"Ðã nh?n tr? phi?u #{id}. Ti?n ph?t: {UiHelper.Tien(phieu.TienPhat)}." : $"Ðã nh?n tr? phi?u #{id} dúng h?n, không phát sinh ti?n ph?t.",
        };
    }

    private static List<SachYeuCau> YeuCauCua(PhieuMuon phieu) => phieu.ChiTietMuons.Select(ct => new SachYeuCau(ct.MaSach, ct.SoLuong)).ToList();

    private static TraSachViewModel TaoFormTraSach(PhieuMuon phieu) => new()
    {
        MaPhieuMuon = phieu.MaPhieuMuon,
        Phieu = phieu,
        SoNgayQuaHan = QuaHanService.TinhSoNgayQuaHan(phieu.HanTra),
        Dong = phieu.ChiTietMuons.Select(ct => new TraSachDong
        {
            MaChiTiet = ct.MaChiTiet, TenSach = ct.Sach?.TenSach ?? "", SoLuong = ct.SoLuong, DonGia = ct.Sach?.DonGia ?? 0, TinhTrangTra = TinhTrangTra.BinhThuong,
        }).ToList(),
    };
}
