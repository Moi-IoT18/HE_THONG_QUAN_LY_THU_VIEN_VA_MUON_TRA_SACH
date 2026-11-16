using TheLoaiEntity = QuanLyThuVien_UNETI01_DHTI17A4HN.Models.TheLoai;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TheLoai;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.TheLoai;

public class TheLoaiService : ITheLoaiService
{
    private readonly ITheLoaiRepository _repository;

    public TheLoaiService(ITheLoaiRepository repository)
    {
        _repository = repository;
    }

    public Task<List<TheLoaiEntity>> GetAllAsync() => _repository.GetAllAsync();

    public Task<TheLoaiEntity?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task<bool> IsDuplicateAsync(string tenTheLoai, int? excludeId = null) => _repository.ExistsAsync(tenTheLoai, excludeId);

    public Task<bool> IsInUseAsync(int id) => _repository.IsInUseAsync(id);

    public async Task AddAsync(TheLoaiEntity entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(TheLoaiEntity entity)
    {
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(TheLoaiEntity entity)
    {
        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync();
    }
}
