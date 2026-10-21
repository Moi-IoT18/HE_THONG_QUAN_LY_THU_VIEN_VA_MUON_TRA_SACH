// Ho va ten: Tran Gia Nghia
// Ma sinh vien: 23103100228
// Noi dung thuc hien: Trien khai Service xu ly nghiep vu Sach (Module 2).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.Sach;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Sach;

public class SachService
{
    private readonly SachRepository _sachRepository;

    public SachService(SachRepository sachRepository)
    {
        _sachRepository = sachRepository;
    }

    public async Task<List<Models.Sach>> LayDanhSachSachAsync()
    {
        return await _sachRepository.GetAllAsync();
    }

    public async Task<Models.Sach?> LaySachTheoIdAsync(int id)
    {
        return await _sachRepository.GetByIdAsync(id);
    }

    public async Task ThemSachAsync(Models.Sach sach)
    {
        await _sachRepository.AddAsync(sach);
    }

    public async Task CapNhatSachAsync(Models.Sach sach)
    {
        await _sachRepository.UpdateAsync(sach);
    }

    public async Task XoaSachAsync(int id)
    {
        await _sachRepository.DeleteAsync(id);
    }
}