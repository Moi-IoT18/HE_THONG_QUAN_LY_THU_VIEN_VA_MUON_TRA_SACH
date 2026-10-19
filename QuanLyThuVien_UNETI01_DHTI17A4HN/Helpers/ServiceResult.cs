namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public class ServiceResult
{
    public bool ThanhCong { get; protected set; }
    public bool KhongTimThay { get; protected set; }
    public string ThongBao { get; set; } = string.Empty;
    public string? LoiGop { get; protected set; }
    public string? FieldName { get; protected set; }

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

    public void ThemLoi(string thongBao, string? fieldName = null)
    {
        ThanhCong = false;
        ThongBao = thongBao;
        FieldName = fieldName;
        LoiGop = string.IsNullOrWhiteSpace(LoiGop)
            ? thongBao
            : $"{LoiGop}{Environment.NewLine}{thongBao}";
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T? DuLieu { get; set; }

    public static ServiceResult<T> Ok(T duLieu, string thongBao = "") => new()
    {
        ThanhCong = true,
        DuLieu = duLieu,
        ThongBao = thongBao,
    };

    public new static ServiceResult<T> Loi(string thongBao, string? fieldName = null) => new()
    {
        ThanhCong = false,
        ThongBao = thongBao,
        FieldName = fieldName,
        LoiGop = thongBao,
    };

    public new static ServiceResult<T> KhongTonTai(string thongBao = "Dữ liệu không tồn tại.") => new()
    {
        KhongTimThay = true,
        ThanhCong = false,
        ThongBao = thongBao,
        LoiGop = thongBao,
    };
}
