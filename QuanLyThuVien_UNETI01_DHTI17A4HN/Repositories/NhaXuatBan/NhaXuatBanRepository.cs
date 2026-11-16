// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Truy vấn nhà xuất bản qua repository dùng chung (Module 2).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

public class NhaXuatBanRepository : Repository<NhaXuatBan>, INhaXuatBanRepository
{
    public NhaXuatBanRepository(AppDbContext db) : base(db)
    {
    }
}
