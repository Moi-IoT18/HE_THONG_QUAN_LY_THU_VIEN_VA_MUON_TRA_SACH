// Họ và tên: Trần Gia Nghĩa
// Mã sinh viên: 23103100228
// Nội dung thực hiện: Lớp phân trang dùng chung.
 
using Microsoft.EntityFrameworkCore;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
 
public class PhanTrang<T> : IPhanTrangInfo
{
    public List<T> Items { get; init; } = new();
    public int TrangHienTai { get; init; }
    public int KichThuocTrang { get; init; }
    public int TongSoDong { get; init; }
 
    public int TongSoTrang => (int)Math.Ceiling(TongSoDong / (double)KichThuocTrang);
    public bool CoTrangTruoc => TrangHienTai > 1;
    public bool CoTrangSau => TrangHienTai < TongSoTrang;
    public int TuDong => TongSoDong == 0 ? 0 : (TrangHienTai - 1) * KichThuocTrang + 1;
    public int DenDong => Math.Min(TrangHienTai * KichThuocTrang, TongSoDong);
 
    public static async Task<PhanTrang<T>> TaoAsync(IQueryable<T> truyVan, int trang, int kichThuocTrang)
    {
        var tong = await truyVan.CountAsync();
        var soTrang = Math.Max(1, (int)Math.Ceiling(tong / (double)kichThuocTrang));
        trang = Math.Clamp(trang, 1, soTrang);
 
        var items = await truyVan
            .Skip((trang - 1) * kichThuocTrang)
            .Take(kichThuocTrang)
            .ToListAsync();
 
        return new PhanTrang<T>
        {
            Items = items,
            TrangHienTai = trang,
            KichThuocTrang = kichThuocTrang,
            TongSoDong = tong,
        };
    }
}
 
public interface IPhanTrangInfo
{
    int TrangHienTai { get; }
    int TongSoTrang { get; }
    bool CoTrangTruoc { get; }
    bool CoTrangSau { get; }
    int TuDong { get; }
    int DenDong { get; }
    int TongSoDong { get; }
}