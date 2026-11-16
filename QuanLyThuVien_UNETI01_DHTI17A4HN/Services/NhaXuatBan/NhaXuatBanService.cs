// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Xử lý nghiệp vụ CRUD nhà xuất bản và bảo vệ dữ liệu sách (Module 2).

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public class NhaXuatBanService : INhaXuatBanService
{
    private readonly INhaXuatBanRepository _repo;

    public NhaXuatBanService(INhaXuatBanRepository repo)
    {
        _repo = repo;
    }

    public Task<List<NhaXuatBan>> GetAllAsync() => _repo.GetAllAsync();

    public Task<NhaXuatBan?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<bool> IsDuplicateAsync(string tenNhaXuatBan, int? excludeId = null)
    {
        var tenDaChuanHoa = tenNhaXuatBan.Trim();
        return _repo.QueryNoTracking().AnyAsync(n =>
            n.TenNhaXuatBan == tenDaChuanHoa
            && (!excludeId.HasValue || n.MaNhaXuatBan != excludeId.Value));
    }

    public Task<bool> IsInUseAsync(int id)
        => _repo.QueryNoTracking().AnyAsync(n => n.MaNhaXuatBan == id && n.Sachs.Any());

    public async Task AddAsync(NhaXuatBan entity)
    {
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(NhaXuatBan entity)
    {
        await _repo.UpdateAsync(entity);
        await _repo.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null || await IsInUseAsync(id)) return false;

        await _repo.DeleteAsync(entity);
        await _repo.SaveChangesAsync();
        return true;
    }
}
