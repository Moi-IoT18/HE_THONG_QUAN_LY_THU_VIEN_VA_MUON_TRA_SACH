// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Dữ liệu mẫu giao dịch – 50 phiếu mượn trải đều ~8 tháng với đủ trạng thái
//                     (Chờ duyệt, Đang mượn, Quá hạn, Đã trả đúng/trễ hạn, Đã hủy, Từ chối)
//                     kèm chi tiết mượn, tiền phạt; sau đó tính lại tồn kho SoLuongCon cho khớp.
 
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Data;
 
public static partial class DbSeeder
{
    private static async Task SeedPhieuMuonAsync(AppDbContext db, Random rd)
    {
        var sachs = await db.Sachs.OrderBy(s => s.MaSach).ToListAsync();
        var banDocs = await db.BanDocs.OrderBy(b => b.MaBanDoc).ToListAsync();
        var nhanVien = await db.TaiKhoans.Where(t => t.VaiTro != VaiTro.BanDoc).Select(t => t.MaTaiKhoan).ToListAsync();
        var sachMuonDuoc = sachs.Where(s => s.TrangThai == TrangThaiSach.ConPhucVu).ToList();
        var sachSeHet = sachMuonDuoc.Where(s => s.TenSach is "Mắt biếc" or "Lập trình Web với ASP.NET Core").ToList();
 
        var now = DateTime.Now;
        var dangGiu = new Dictionary<int, HashSet<int>>(); // MaBanDoc -> các MaSach trong phiếu chưa trả
        var phieus = new List<PhieuMuon>();
 
        DateTime Gio(DateTime ngay) => ngay.Date.AddHours(rd.Next(8, 17)).AddMinutes(rd.Next(0, 60));
        int NhanVien() => nhanVien[rd.Next(nhanVien.Count)];
 
        List<Sach> ChonSach(int maBanDoc, int soCuon, bool giuSach, List<Sach>? batBuoc = null)
        {
            var daGiu = dangGiu.TryGetValue(maBanDoc, out var set) ? set : new HashSet<int>();
            var chon = new List<Sach>(batBuoc ?? []);
            var ungVien = sachMuonDuoc
                .Where(s => !daGiu.Contains(s.MaSach) && !chon.Contains(s) && !sachSeHet.Contains(s))
                .OrderBy(_ => rd.Next())
                .ToList();
            var conLai = Math.Min(soCuon - chon.Count, QuyDinh.SoSachToiDa - daGiu.Count - chon.Count);
            chon.AddRange(ungVien.Take(Math.Max(0, conLai)));
            if (giuSach)
            {
                if (!dangGiu.ContainsKey(maBanDoc)) dangGiu[maBanDoc] = new HashSet<int>();
                foreach (var s in chon) dangGiu[maBanDoc].Add(s.MaSach);
            }
            return chon;
        }
 
        PhieuMuon TaoPhieu(BanDoc bd, TrangThaiPhieu tt, DateTime ngayMuon, List<Sach> ds)
        {
            var phieu = new PhieuMuon
            {
                MaBanDoc = bd.MaBanDoc,
                NgayMuon = ngayMuon,
                HanTra = ngayMuon.Date.AddDays(QuyDinh.SoNgayMuon),
                TrangThai = tt,
            };
            foreach (var s in ds)
            {
                phieu.ChiTietMuons.Add(new ChiTietMuon
                {
                    MaSach = s.MaSach,
                    SoLuong = 1,
                    TinhTrangMuon = rd.Next(5) == 0 ? TinhTrangMuon.CuNhung : TinhTrangMuon.BinhThuong,
                });
            }
            if (tt is not TrangThaiPhieu.ChoDuyet and not TrangThaiPhieu.DaHuy)
            {
                phieu.NhanVienXuLy = NhanVien();
                phieu.NgayXuLy = ngayMuon.AddHours(rd.Next(1, 5));
            }
            phieus.Add(phieu);
            return phieu;
        }
 
        void TraSach(PhieuMuon phieu, DateTime ngayTra, TinhTrangTra? tinhTrangDacBiet = null)
        {
            phieu.NgayTra = ngayTra;
            var soNgay = QuaHanService.TinhSoNgayQuaHan(phieu.HanTra, ngayTra);
            var dau = true;
            foreach (var ct in phieu.ChiTietMuons)
            {
                var sach = sachs.First(s => s.MaSach == ct.MaSach);
                ct.TinhTrangTra = dau && tinhTrangDacBiet.HasValue ? tinhTrangDacBiet.Value : TinhTrangTra.BinhThuong;
                ct.SoNgayQuaHan = soNgay;
                ct.TienPhat = TinhPhatService.TinhTienPhat(soNgay, ct.SoLuong, sach.DonGia, ct.TinhTrangTra.Value);
                dau = false;
            }
            phieu.TienPhat = phieu.ChiTietMuons.Sum(c => c.TienPhat);
        }
 
        // ---------- 1. Bạn đọc bandoc01: bộ phiếu cố định để demo ----------
        var bd01 = banDocs[0];
        var p = TaoPhieu(bd01, TrangThaiPhieu.DaTra, Gio(now.AddDays(-75)), ChonSach(bd01.MaBanDoc, 2, false));
        TraSach(p, Gio(now.AddDays(-66)));
        p = TaoPhieu(bd01, TrangThaiPhieu.DaTra, Gio(now.AddDays(-40)), ChonSach(bd01.MaBanDoc, 1, false));
        TraSach(p, Gio(now.AddDays(-23))); // trễ 3 ngày
        TaoPhieu(bd01, TrangThaiPhieu.DangMuon, Gio(now.AddDays(-6)), ChonSach(bd01.MaBanDoc, 2, true, sachSeHet));
        p = TaoPhieu(bd01, TrangThaiPhieu.DaHuy, Gio(now.AddDays(-30)), ChonSach(bd01.MaBanDoc, 1, false));
        p.GhiChu = "Bạn đọc tự hủy";
 
        // ---------- 2. Phiếu đã trả (lịch sử) ----------
        for (var i = 0; i < 22; i++)
        {
            var bd = banDocs[rd.Next(1, 28)];
            var ngayMuon = Gio(now.AddDays(-rd.Next(20, 240)));
            var phieu = TaoPhieu(bd, TrangThaiPhieu.DaTra, ngayMuon, ChonSach(bd.MaBanDoc, rd.Next(1, 4), false));
            var ngayTra = Gio(ngayMuon.AddDays(rd.Next(3, 21)));
            if (ngayTra > now) ngayTra = now.AddHours(-1);
            TinhTrangTra? dacBiet = i switch { 3 => TinhTrangTra.HuHong, 9 => TinhTrangTra.Mat, 15 => TinhTrangTra.HuHong, _ => null };
            TraSach(phieu, ngayTra, dacBiet);
            if (dacBiet == TinhTrangTra.Mat) phieu.GhiChu = "Bạn đọc làm mất 1 cuốn, đã bồi thường";
        }
 
        // ---------- 3. Đang mượn (chưa đến hạn) ----------
        for (var i = 0; i < 8; i++)
        {
            var bd = banDocs[rd.Next(1, 15)];
            TaoPhieu(bd, TrangThaiPhieu.DangMuon, Gio(now.AddDays(-rd.Next(1, 13))), ChonSach(bd.MaBanDoc, rd.Next(1, 3), true));
        }
 
        // 1 phiếu Đang mượn nhưng đã qua hạn trả – chưa được quét (để demo chức năng "Quét quá hạn")
        var bdQuet = banDocs[15];
        TaoPhieu(bdQuet, TrangThaiPhieu.DangMuon, Gio(now.AddDays(-17)), ChonSach(bdQuet.MaBanDoc, 1, true));
 
        // ---------- 4. Quá hạn ----------
        for (var i = 0; i < 6; i++)
        {
            var bd = banDocs[16 + i % 5];
            TaoPhieu(bd, TrangThaiPhieu.QuaHan, Gio(now.AddDays(-rd.Next(16, 45))), ChonSach(bd.MaBanDoc, rd.Next(1, 3), true));
        }
 
        // ---------- 5. Chờ duyệt ----------
        for (var i = 0; i < 5; i++)
        {
            var bd = banDocs[rd.Next(1, 15)];
            var ds = ChonSach(bd.MaBanDoc, rd.Next(1, 3), true);
            if (ds.Count > 0)
            {
                TaoPhieu(bd, TrangThaiPhieu.ChoDuyet, now.AddDays(-rd.Next(0, 3)).AddHours(-rd.Next(1, 8)), ds);
            }
        }
 
        // ---------- 6. Đã hủy / Từ chối ----------
        string[] lyDoTuChoi = ["Sách đã được bạn đọc khác mượn hết", "Bạn đọc chưa trả sách quá hạn", "Thông tin thẻ chưa được xác minh"];
        for (var i = 0; i < 3; i++)
        {
            var bd = banDocs[rd.Next(1, 28)];
            var huy = TaoPhieu(bd, TrangThaiPhieu.DaHuy, Gio(now.AddDays(-rd.Next(5, 150))), ChonSach(bd.MaBanDoc, 1, false));
            huy.GhiChu = "Bạn đọc tự hủy";
 
            bd = banDocs[rd.Next(1, 28)];
            var tuChoi = TaoPhieu(bd, TrangThaiPhieu.TuChoi, Gio(now.AddDays(-rd.Next(5, 150))), ChonSach(bd.MaBanDoc, rd.Next(1, 3), false));
            tuChoi.GhiChu = "Từ chối: " + lyDoTuChoi[i];
        }
 
        // Lưu theo thứ tự thời gian để mã phiếu tăng dần theo ngày mượn
        db.PhieuMuons.AddRange(phieus.Where(x => x.ChiTietMuons.Count > 0).OrderBy(x => x.NgayMuon));
 
        // ---------- 7. Tính lại tồn kho cho khớp với các phiếu đang giữ sách ----------
        var dangMuon = phieus
            .Where(x => x.TrangThai is TrangThaiPhieu.DangMuon or TrangThaiPhieu.QuaHan)
            .SelectMany(x => x.ChiTietMuons)
            .GroupBy(ct => ct.MaSach)
            .ToDictionary(g => g.Key, g => g.Sum(ct => ct.SoLuong));
 
        foreach (var sach in sachs)
        {
            var soDangMuon = dangMuon.GetValueOrDefault(sach.MaSach);
            if (sachSeHet.Contains(sach))
            {
                sach.SoLuong = Math.Max(1, soDangMuon); // cố ý để hết sách
            }
            sach.SoLuong = Math.Max(sach.SoLuong, soDangMuon);
            sach.SoLuongCon = sach.SoLuong - soDangMuon;
        }
 
        await db.SaveChangesAsync();
    }
}
