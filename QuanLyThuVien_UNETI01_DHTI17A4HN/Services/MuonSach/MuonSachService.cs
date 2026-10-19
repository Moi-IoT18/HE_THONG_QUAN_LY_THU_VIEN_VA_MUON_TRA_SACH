// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Kiểm tra điều kiện mượn sách bằng LINQ (mục 7 và 8.3 của đề).
//                     Dùng chung cho: bạn đọc tạo phiếu (SV3) và thủ thư duyệt phiếu (SV4).
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class MuonSachService : IMuonSachService
{
    private readonly IBanDocRepository _banDocRepo;
    private readonly IPhieuMuonRepository _phieuRepo;
    private readonly IRepository<Sach> _sachRepo;
 
    public MuonSachService(IBanDocRepository banDocRepo, IPhieuMuonRepository phieuRepo, IRepository<Sach> sachRepo)
    {
        _banDocRepo = banDocRepo;
        _phieuRepo = phieuRepo;
        _sachRepo = sachRepo;
    }
 
    public async Task<List<string>> KiemTraDieuKienAsync(int maBanDoc, IReadOnlyCollection<SachYeuCau> dsSach, int? maPhieuBoQua = null)
    {
        var loi = new List<string>();
 
        // 1. Bạn đọc tồn tại, tài khoản hoạt động, thẻ còn hạn
        var banDoc = await _banDocRepo.LayKemTaiKhoanAsync(maBanDoc);
        if (banDoc is null)
        {
            loi.Add("Bạn đọc không tồn tại.");
            return loi;
        }
        if (banDoc.TaiKhoan is { TrangThai: false })
        {
            loi.Add("Tài khoản của bạn đọc đang bị khóa.");
        }
        if (banDoc.TrangThai != TrangThaiBanDoc.HoatDong)
        {
            loi.Add("Thẻ bạn đọc đang bị tạm khóa.");
        }
        if (banDoc.HanThe.Date < DateTime.Today)
        {
            loi.Add($"Thẻ bạn đọc đã hết hạn từ ngày {banDoc.HanThe:dd/MM/yyyy}.");
        }
 
        // 2. Danh sách sách hợp lệ
        if (dsSach.Count == 0)
        {
            loi.Add("Phiếu mượn phải có ít nhất 1 cuốn sách.");
            return loi;
        }
        if (dsSach.Any(s => s.SoLuong <= 0))
        {
            loi.Add("Số lượng mượn mỗi sách phải lớn hơn 0.");
        }
        if (dsSach.GroupBy(s => s.MaSach).Any(g => g.Count() > 1))
        {
            loi.Add("Một đầu sách chỉ được xuất hiện 1 lần trong phiếu.");
        }
 
        // 3. Bạn đọc đang có phiếu quá hạn chưa trả thì không được mượn thêm
        if (await _phieuRepo.CoPhieuQuaHanAsync(maBanDoc, maPhieuBoQua))
        {
            loi.Add("Bạn đọc đang có phiếu quá hạn chưa trả, cần trả sách trước khi mượn thêm.");
        }
 
        // 4. Từng sách: tồn tại, còn phục vụ, còn đủ số lượng
        var maSachs = dsSach.Select(s => s.MaSach).Distinct().ToList();
        var sachs = await _sachRepo.QueryNoTracking().Where(s => maSachs.Contains(s.MaSach)).ToListAsync();
        foreach (var yc in dsSach)
        {
            var sach = sachs.FirstOrDefault(s => s.MaSach == yc.MaSach);
            if (sach is null)
            {
                loi.Add($"Sách mã {yc.MaSach} không tồn tại.");
            }
            else if (sach.TrangThai != TrangThaiSach.ConPhucVu)
            {
                loi.Add($"Sách \"{sach.TenSach}\" đã ngừng phục vụ.");
            }
            else if (sach.SoLuongCon < yc.SoLuong)
            {
                loi.Add(sach.SoLuongCon == 0
                    ? $"Sách \"{sach.TenSach}\" đã hết, không thể mượn."
                    : $"Sách \"{sach.TenSach}\" chỉ còn {sach.SoLuongCon} cuốn.");
            }
        }
 
        // 5. Không mượn trùng: sách đã nằm trong phiếu chưa trả của bạn đọc
        var phieuDangGiu = _phieuRepo.TruyVanPhieuDangGiu(maBanDoc, maPhieuBoQua);
 
        var sachTrung = await phieuDangGiu
            .SelectMany(p => p.ChiTietMuons)
            .Where(ct => maSachs.Contains(ct.MaSach))
            .Select(ct => ct.Sach!.TenSach)
            .Distinct()
            .ToListAsync();
        foreach (var ten in sachTrung)
        {
            loi.Add($"Bạn đọc đã có sách \"{ten}\" trong một phiếu chưa trả (không được mượn trùng).");
        }
 
        // 6. Giới hạn tổng số cuốn đang giữ
        var dangGiu = await phieuDangGiu.SelectMany(p => p.ChiTietMuons).SumAsync(ct => (int?)ct.SoLuong) ?? 0;
        var muonThem = dsSach.Sum(s => s.SoLuong);
        if (dangGiu + muonThem > QuyDinh.SoSachToiDa)
        {
            loi.Add($"Vượt giới hạn {QuyDinh.SoSachToiDa} cuốn: bạn đọc đang giữ {dangGiu} cuốn, muốn mượn thêm {muonThem} cuốn.");
        }
 
        return loi;
    }
}
