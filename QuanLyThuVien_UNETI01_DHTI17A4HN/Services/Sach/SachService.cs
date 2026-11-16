// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Xử lý nghiệp vụ sách, bộ lọc và phân trang (Module 2).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public class SachService : ISachService
{
    private readonly ISachRepository _sachRepository;

    public SachService(ISachRepository sachRepository)
    {
        _sachRepository = sachRepository;
    }

    public async Task<SachDanhSachViewModel> LayDanhSachSachAsync(SachFilterViewModel filter, int trang)
    {
        var truyVan = _sachRepository.TruyVanDanhSach(filter.TuKhoa, filter.MaTheLoai,
            filter.NamXuatBan, filter.TrangThai, filter.ConSach, filter.GiaTu, filter.GiaDen, filter.SapXep);
        var danhSach = await PhanTrang<Models.Sach>.TaoAsync(truyVan, trang, QuyDinh.KichThuocTrang);
        filter.TuKhoa = filter.TuKhoa?.Trim();
        return new SachDanhSachViewModel { Filter = filter, DanhSach = danhSach };
    }

    public async Task<Models.Sach?> LaySachTheoIdAsync(int id)
    {
        return await _sachRepository.GetByIdAsync(id);
    }

    public async Task ThemSachAsync(Models.Sach sach)
    {
        await _sachRepository.AddAsync(sach);
        await _sachRepository.SaveChangesAsync();
    }

    public async Task CapNhatSachAsync(Models.Sach sach)
    {
        await _sachRepository.UpdateAsync(sach);
        await _sachRepository.SaveChangesAsync();
    }

    public Task<bool> KhoaNgoaiHopLeAsync(int maTheLoai, int maNhaXuatBan, bool choPhepNgungHoatDong = false)
        => _sachRepository.KhoaNgoaiHopLeAsync(maTheLoai, maNhaXuatBan, choPhepNgungHoatDong);

    public async Task<bool> XoaSachAsync(int id)
    {
        var sach = await _sachRepository.GetByIdAsync(id);
        if (sach is null) return false;
        if (await _sachRepository.IsInUseAsync(id)) return false;

        await _sachRepository.DeleteAsync(sach);
        await _sachRepository.SaveChangesAsync();
        return true;
    }
}