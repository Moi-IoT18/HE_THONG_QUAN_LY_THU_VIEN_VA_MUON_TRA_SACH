using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;
using TaiKhoanModel = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TaiKhoan;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TaiKhoan;

public class TaiKhoanService : ITaiKhoanService
{
    private readonly ITaiKhoanRepository _repository;

    public TaiKhoanService(ITaiKhoanRepository repository)
    {
        _repository = repository;
    }

    public Task<List<TaiKhoanModel>> GetAllAsync() => _repository.GetAllAsync();

    public Task<TaiKhoanModel?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task<bool> IsDuplicateAsync(string tenDangNhap, int? excludeId = null)
        => _repository.ExistsAsync(tenDangNhap, excludeId);

    public async Task AddAsync(TaiKhoanModel taiKhoan)
    {
        await _repository.AddAsync(taiKhoan);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaiKhoanModel taiKhoan)
    {
        await _repository.UpdateAsync(taiKhoan);
        await _repository.SaveChangesAsync();
    }
}
