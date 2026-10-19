// H? và tên: Tr?nh Th? Thanh Tâm
// Mã sinh viên: 23103100193
// N?i dung th?c hi?n: Module 4 – Ki?m soát lu?ng tr?ng thái phi?u mu?n – ch? cho phép chuy?n theo
//                     dúng so d? (m?c 8.4 c?a d?), không cho c?p nh?t tùy ý.

using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public static class TrangThaiPhieuService
{
    private static readonly Dictionary<TrangThaiPhieu, TrangThaiPhieu[]> LuongHopLe = new()
    {
        [TrangThaiPhieu.ChoDuyet] = [TrangThaiPhieu.DangMuon, TrangThaiPhieu.TuChoi, TrangThaiPhieu.DaHuy],
        [TrangThaiPhieu.DangMuon] = [TrangThaiPhieu.QuaHan, TrangThaiPhieu.DaTra],
        [TrangThaiPhieu.QuaHan] = [TrangThaiPhieu.DaTra],
        [TrangThaiPhieu.DaTra] = [],
        [TrangThaiPhieu.DaHuy] = [],
        [TrangThaiPhieu.TuChoi] = [],
    };

    public static bool CoTheChuyen(TrangThaiPhieu tu, TrangThaiPhieu sang)
        => LuongHopLe.TryGetValue(tu, out var dich) && dich.Contains(sang);

    public static bool LaTrangThaiCuoi(TrangThaiPhieu tt) => LuongHopLe[tt].Length == 0;

    public static bool DangXuLy(TrangThaiPhieu tt) => tt is TrangThaiPhieu.DangMuon or TrangThaiPhieu.QuaHan;

    public static string ThongBaoKhongHopLe(TrangThaiPhieu tu, TrangThaiPhieu sang)
        => $"Không th? chuy?n phi?u t? \"{tu.TenHienThi()}\" sang \"{sang.TenHienThi()}\".";
}
