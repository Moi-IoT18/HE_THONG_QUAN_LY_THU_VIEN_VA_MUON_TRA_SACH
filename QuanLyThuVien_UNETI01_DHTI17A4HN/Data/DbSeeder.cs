// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Khởi tạo dataset demo đầy đủ và không phá hủy cho kiểm thử Modules 1-5.

using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

public static partial class DbSeeder
{
    public static async Task SeedDemoAsync(
        AppDbContext db, string matKhauAdmin, string matKhauThuThu, string matKhauBanDoc)
    {
        KiemTraMatKhauDemo(matKhauAdmin, nameof(matKhauAdmin));
        KiemTraMatKhauDemo(matKhauThuThu, nameof(matKhauThuThu));
        KiemTraMatKhauDemo(matKhauBanDoc, nameof(matKhauBanDoc));

        if (await db.TaiKhoans.AnyAsync()
            || await db.TheLoais.AnyAsync()
            || await db.NhaXuatBans.AnyAsync()
            || await db.Sachs.AnyAsync()
            || await db.BanDocs.AnyAsync()
            || await db.PhieuMuons.AnyAsync()
            || await db.ChiTietMuons.AnyAsync())
        {
            throw new InvalidOperationException(
                "Demo seeding only runs when all library tables are empty; existing data was preserved.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        var admins = new[]
        {
            TaoTaiKhoan("admin01", matKhauAdmin, "Quản trị viên 01", "admin01@demo.uneti.edu.vn", VaiTro.QuanLy),
            TaoTaiKhoan("admin02", matKhauAdmin, "Quản trị viên 02", "admin02@demo.uneti.edu.vn", VaiTro.QuanLy),
        };
        var nhanViens = new[]
        {
            TaoTaiKhoan("nhanvien01", matKhauThuThu, "Nhân viên thư viện 01", "nhanvien01@demo.uneti.edu.vn", VaiTro.ThuThu),
            TaoTaiKhoan("nhanvien02", matKhauThuThu, "Nhân viên thư viện 02", "nhanvien02@demo.uneti.edu.vn", VaiTro.ThuThu),
        };
        db.TaiKhoans.AddRange(admins);
        db.TaiKhoans.AddRange(nhanViens);
        await db.SaveChangesAsync();

        var theLoais = new[]
        {
            new TheLoai { TenTheLoai = "Công nghệ thông tin", MoTa = "Máy tính, lập trình và công nghệ", TrangThai = true, ThuTuHienThi = 1 },
            new TheLoai { TenTheLoai = "Khoa học tự nhiên", MoTa = "Toán học, vật lý và khoa học tự nhiên", TrangThai = true, ThuTuHienThi = 2 },
            new TheLoai { TenTheLoai = "Kinh tế - Quản lý", MoTa = "Kinh tế, quản trị và tài chính", TrangThai = true, ThuTuHienThi = 3 },
            new TheLoai { TenTheLoai = "Văn học", MoTa = "Tác phẩm văn học và nghệ thuật", TrangThai = true, ThuTuHienThi = 4 },
            new TheLoai { TenTheLoai = "Kỹ năng - Tham khảo", MoTa = "Kỹ năng học tập và sách tham khảo", TrangThai = true, ThuTuHienThi = 5 },
        };
        var nhaXuatBans = new[]
        {
            new NhaXuatBan { TenNhaXuatBan = "NXB Giáo Dục Việt Nam", DiaChi = "Hà Nội", SoDienThoai = "02438220801", Email = "contact@nxbgd.vn", TrangThai = true },
            new NhaXuatBan { TenNhaXuatBan = "NXB Khoa Học và Kỹ Thuật", DiaChi = "Hà Nội", SoDienThoai = "02439423362", Email = "contact@khkt.vn", TrangThai = true },
            new NhaXuatBan { TenNhaXuatBan = "NXB Trẻ", DiaChi = "Thành phố Hồ Chí Minh", SoDienThoai = "02839316289", Email = "contact@nxbtre.vn", TrangThai = true },
        };
        db.TheLoais.AddRange(theLoais);
        db.NhaXuatBans.AddRange(nhaXuatBans);
        await db.SaveChangesAsync();

        await SeedBanDocAsync(db, new Random(316), matKhauBanDoc);
        var readers = await db.BanDocs.OrderBy(b => b.MaBanDoc).ToListAsync();
        var books = Enumerable.Range(1, 15).Select(i => new Sach
        {
            TenSach = TenSachDemo[i - 1],
            TacGia = $"Tác giả mẫu {i:00}",
            MaTheLoai = theLoais[(i - 1) % theLoais.Length].MaTheLoai,
            MaNhaXuatBan = nhaXuatBans[(i - 1) % nhaXuatBans.Length].MaNhaXuatBan,
            NamXuatBan = 2018 + i % 9,
            SoLuong = 20 + i % 16,
            SoLuongCon = 20 + i % 16,
            DonGia = 50000m + i * 12500m,
            ViTriKeSach = $"Kệ {(char)('A' + (i - 1) % 5)}-{i:00}",
            TrangThai = i == 15 ? TrangThaiSach.NgungPhucVu : TrangThaiSach.ConPhucVu,
            GhiChu = "Bản ghi minh họa phục vụ kiểm thử.",
        }).ToArray();
        db.Sachs.AddRange(books);
        await db.SaveChangesAsync();

        var now = DateTime.Now;
        var random = new Random(316);
        for (var i = 1; i <= 50; i++)
        {
            var status = TrangThaiDemo(i);
            var issue = now.AddDays(-SoNgayTuNgayTao(i)).AddHours(-random.Next(1, 12));
            var due = issue.Date.AddDays(14);
            var delay = status == TrangThaiPhieu.DaTra && i % 3 == 0 ? 1 + i % 8 : 0;
            var returnAt = status == TrangThaiPhieu.DaTra
                ? due.AddDays(delay).AddHours(10)
                : (DateTime?)null;
            var isCurrentLoan = status is TrangThaiPhieu.DangMuon or TrangThaiPhieu.QuaHan;
            var bookIndex = isCurrentLoan ? (i - 11) % 14 : (i - 1) % books.Length;
            var loan = new PhieuMuon
            {
                MaBanDoc = readers[(i - 1) % 25].MaBanDoc,
                NgayMuon = issue,
                HanTra = due,
                NgayTra = returnAt,
                TrangThai = status,
                GhiChu = status == TrangThaiPhieu.DaHuy ? "Phiếu hủy mẫu." : null,
                NhanVienXuLy = status is TrangThaiPhieu.DangMuon or TrangThaiPhieu.QuaHan or TrangThaiPhieu.DaTra or TrangThaiPhieu.DaHuy
                    ? nhanViens[(i - 1) % nhanViens.Length].MaTaiKhoan
                    : null,
                NgayXuLy = status == TrangThaiPhieu.ChoDuyet
                    ? null
                    : status == TrangThaiPhieu.DaTra ? returnAt : issue.AddHours(2),
            };

            var quantity = i % 5 == 0 ? 2 : 1;
            var book = books[bookIndex];
            var detail = new ChiTietMuon
            {
                Sach = book,
                SoLuong = quantity,
                TinhTrangMuon = TinhTrangMuon.BinhThuong,
                TinhTrangTra = status == TrangThaiPhieu.DaTra
                    ? status == TrangThaiPhieu.DaTra && i % 10 == 0 ? TinhTrangTra.Hong : TinhTrangTra.BinhThuong
                    : null,
                SoNgayQuaHan = status == TrangThaiPhieu.DaTra ? delay : 0,
                TienPhat = status == TrangThaiPhieu.DaTra ? delay * QuyDinh.PhatQuaHanMoiNgay * quantity : 0,
                GhiChu = status == TrangThaiPhieu.DaTra ? "Đã kiểm tra khi nhận trả." : null,
            };
            loan.ChiTietMuons.Add(detail);

            if (i % 4 == 0)
            {
                var secondBook = books[(bookIndex + 1) % (isCurrentLoan ? 14 : books.Length)];
                var second = new ChiTietMuon
                {
                    Sach = secondBook,
                    SoLuong = 1,
                    TinhTrangMuon = TinhTrangMuon.BinhThuong,
                    TinhTrangTra = status == TrangThaiPhieu.DaTra ? TinhTrangTra.BinhThuong : null,
                    SoNgayQuaHan = status == TrangThaiPhieu.DaTra ? delay : 0,
                    TienPhat = status == TrangThaiPhieu.DaTra ? delay * QuyDinh.PhatQuaHanMoiNgay : 0,
                    GhiChu = status == TrangThaiPhieu.DaTra ? "Đã kiểm tra khi nhận trả." : null,
                };
                loan.ChiTietMuons.Add(second);
            }

            loan.TienPhat = loan.ChiTietMuons.Sum(ct => ct.TienPhat);
            db.PhieuMuons.Add(loan);

            if (isCurrentLoan)
            {
                foreach (var held in loan.ChiTietMuons)
                    held.Sach!.SoLuongCon -= held.SoLuong;
            }

            foreach (var returned in loan.ChiTietMuons.Where(ct =>
                ct.TinhTrangTra is TinhTrangTra.Hong or TinhTrangTra.Mat))
            {
                returned.Sach!.SoLuong -= returned.SoLuong;
            }
        }

        foreach (var book in books)
        {
            if (book.SoLuongCon < 0 || book.SoLuongCon > book.SoLuong)
                throw new InvalidOperationException($"Generated inventory for '{book.TenSach}' is inconsistent.");
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static TrangThaiPhieu TrangThaiDemo(int index) => index switch
    {
        <= 10 => TrangThaiPhieu.ChoDuyet,
        <= 25 => TrangThaiPhieu.DangMuon,
        <= 33 => TrangThaiPhieu.QuaHan,
        <= 48 => TrangThaiPhieu.DaTra,
        _ => TrangThaiPhieu.DaHuy,
    };

    private static int SoNgayTuNgayTao(int index) => index switch
    {
        <= 10 => 1 + index % 6,
        <= 25 => 2 + index % 8,
        <= 33 => 35 + index % 50,
        <= 48 => 25 + index % 25,
        _ => 5 + index,
    };

    private static readonly string[] TenSachDemo =
    [
        "Nhập môn lập trình C#",
        "Cơ sở dữ liệu quan hệ",
        "Phát triển ứng dụng web",
        "Cấu trúc dữ liệu căn bản",
        "Mạng máy tính đại cương",
        "Toán cao cấp ứng dụng",
        "Vật lý trong đời sống",
        "Khoa học dữ liệu nhập môn",
        "Nguyên lý quản trị",
        "Kinh tế học căn bản",
        "Tài chính doanh nghiệp",
        "Văn học Việt Nam chọn lọc",
        "Truyện ngắn và ký",
        "Kỹ năng học tập đại học",
        "Phương pháp nghiên cứu khoa học",
    ];
}
