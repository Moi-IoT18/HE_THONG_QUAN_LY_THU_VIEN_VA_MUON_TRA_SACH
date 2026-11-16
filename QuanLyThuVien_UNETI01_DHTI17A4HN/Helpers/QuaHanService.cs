namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class QuaHanService
{
    public static int TinhSoNgayQuaHan(DateTime hanTra, DateTime? ngayHienTai = null)
    {
        var now = (ngayHienTai ?? DateTime.Today).Date;
        if (hanTra.Date >= now) return 0;
        return (now - hanTra.Date).Days;
    }

    public static decimal TinhTienPhatTamTinh(
        DateTime hanTra, int soLuong, DateTime? ngayHienTai = null)
    {
        if (soLuong < 0)
            throw new ArgumentOutOfRangeException(nameof(soLuong), "Số lượng không được âm.");

        return TinhSoNgayQuaHan(hanTra, ngayHienTai) * QuyDinh.PhatQuaHanMoiNgay * soLuong;
    }
}
