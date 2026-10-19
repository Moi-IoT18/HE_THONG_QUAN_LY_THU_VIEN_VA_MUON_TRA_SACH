// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Module 2 – Nghiệp vụ Nhà xuất bản: không trùng tên, xóa an toàn (chặn khi đã có sách).
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class NhaXuatBanService : INhaXuatBanService
{
    private readonly INhaXuatBanRepository _repo;
 
    public NhaXuatBanService(INhaXuatBanRepository repo)
    {
        _repo = repo;
    }
 
    public async Task<NhaXuatBanIndexViewModel> LayDanhSachAsync(string? tuKhoa, bool? trangThai, int trang)
        => new()
        {
            TuKhoa = tuKhoa,
            TrangThai = trangThai,
            DanhSach = await PhanTrang<NhaXuatBanDong>.TaoAsync(_repo.TruyVanDanhSach(tuKhoa, trangThai), trang, QuyDinh.KichThuocTrang),
        };
 
    public Task<NhaXuatBan?> LayChiTietAsync(int id) => _repo.LayChiTietAsync(id);
    public Task<NhaXuatBan?> LayTheoMaAsync(int id) => _repo.GetByIdAsync(id);
    public Task<int> DemSachAsync(int id) => _repo.DemSachAsync(id);
 
    public async Task<ServiceResult> TaoAsync(NhaXuatBan nxb)
    {
        if (await _repo.TrungTenAsync(nxb.TenNhaXuatBan))
            return ServiceResult.Loi("Tên nhà xuất bản đã tồn tại", nameof(NhaXuatBan.TenNhaXuatBan));
 
        nxb.TenNhaXuatBan = nxb.TenNhaXuatBan.Trim();
        await _repo.AddAsync(nxb);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã thêm nhà xuất bản \"{nxb.TenNhaXuatBan}\".");
    }
 
    public async Task<ServiceResult> CapNhatAsync(int id, NhaXuatBan nxb)
    {
        var nxbDb = await _repo.GetByIdAsync(id);
        if (nxbDb is null) return ServiceResult.KhongTonTai();
        if (await _repo.TrungTenAsync(nxb.TenNhaXuatBan, id))
            return ServiceResult.Loi("Tên nhà xuất bản đã tồn tại", nameof(NhaXuatBan.TenNhaXuatBan));
 
        nxbDb.TenNhaXuatBan = nxb.TenNhaXuatBan.Trim();
        nxbDb.DiaChi = nxb.DiaChi;
        nxbDb.SoDienThoai = nxb.SoDienThoai;
        nxbDb.Email = nxb.Email;
        nxbDb.TrangThai = nxb.TrangThai;
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã cập nhật nhà xuất bản \"{nxbDb.TenNhaXuatBan}\".");
    }
 
    public async Task<ServiceResult> XoaAsync(int id)
    {
        var nxb = await _repo.GetByIdAsync(id);
        if (nxb is null) return ServiceResult.KhongTonTai();
 
        if (await _repo.DemSachAsync(id) > 0)
            return ServiceResult.Loi($"Không thể xóa \"{nxb.TenNhaXuatBan}\" vì đã có sách tham chiếu. Hãy chuyển sang ngừng hợp tác.");
 
        _repo.Remove(nxb);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã xóa nhà xuất bản \"{nxb.TenNhaXuatBan}\".");
    }
}
