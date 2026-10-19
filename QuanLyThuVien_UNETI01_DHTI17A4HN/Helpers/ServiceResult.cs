namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public class ServiceResult
{
    public bool ThanhCong { get; private set; }
    public bool KhongTimThay { get; private set; }
    public string ThongBao { get; private set; } = string.Empty;
    public string? LoiGop { get; private set; }
    public string? FieldName { get; private set; }

    public static ServiceResult Ok(string thongBao) => new() { ThanhCong = true, ThongBao = thongBao };

    public static ServiceResult Loi(string thongBao, string? fieldName = null) => new()
    {
        ThanhCong = false,
        ThongBao = thongBao,
        FieldName = fieldName,
        LoiGop = thongBao,
    };

    public static ServiceResult KhongTonTai(string thongBao = "Dữ liệu không tồn tại.") => new()
    {
        KhongTimThay = true,
        ThanhCong = false,
        ThongBao = thongBao,
        LoiGop = thongBao,
    };
}
