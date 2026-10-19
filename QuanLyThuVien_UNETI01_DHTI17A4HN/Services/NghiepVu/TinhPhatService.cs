// H? v� t�n: Tr?nh Th? Thanh T�m
// M� sinh vi�n: 23103100193
// N?i dung th?c hi?n: Module 4 � C�ng th?c t�nh ti?n ph?t khi tr? s�ch (QD03, QD04, QD05).

using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public static class TinhPhatService
{
    public static decimal TinhTienPhat(int soNgayQuaHan, int soLuong, decimal donGia, TinhTrangTra tinhTrang)
    {
        var phatQuaHan = soNgayQuaHan * QuyDinh.PhatQuaHanMoiNgay * soLuong;
        var phatTinhTrang = tinhTrang switch
        {
            TinhTrangTra.HuHong => donGia * QuyDinh.TyLePhatHuHong * soLuong,
            TinhTrangTra.Mat => donGia * QuyDinh.TyLePhatMat * soLuong,
            _ => 0m,
        };
        return Math.Round(phatQuaHan + phatTinhTrang, 0);
    }

    public static decimal PhatDuKien(PhieuMuon phieu)
    {
        var soNgay = QuaHanService.TinhSoNgayQuaHan(phieu.HanTra);
        var soCuon = phieu.ChiTietMuons.Sum(ct => ct.SoLuong);
        return soNgay * QuyDinh.PhatQuaHanMoiNgay * soCuon;
    }
}
