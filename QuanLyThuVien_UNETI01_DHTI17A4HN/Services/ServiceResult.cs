namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;

public class ServiceResult
{
    public bool ThanhCong => string.IsNullOrWhiteSpace(LoiGop) && !KhongTimThay;
    public bool KhongTimThay { get; protected set; }
    public string? ThongBao { get; protected set; }
    public string LoiGop { get; protected set; } = string.Empty;
    public List<(string FieldName, string Error)> LoiTheoTruong { get; } = new();

    public void ThemLoi(string message, string? fieldName = null)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        if (string.IsNullOrWhiteSpace(fieldName))
        {
            if (string.IsNullOrWhiteSpace(LoiGop))
                LoiGop = message;
            else
                LoiGop += "\n" + message;
            return;
        }

        LoiTheoTruong.Add((fieldName, message));
    }

    public static ServiceResult Ok(string? thongBao = null) => new() { ThongBao = thongBao };
    public static ServiceResult Loi(string message) => new() { LoiGop = message };
    public static ServiceResult KhongTonTai() => new() { KhongTimThay = true, ThongBao = "Dữ liệu không tồn tại." };
}

public class ServiceResult<T> : ServiceResult
{
    public T? DuLieu { get; private set; }

    public static ServiceResult<T> Ok(T data, string? thongBao = null) => new() { DuLieu = data, ThongBao = thongBao };
    public static new ServiceResult<T> Loi(string message) => new() { LoiGop = message };
    public static new ServiceResult<T> KhongTonTai() => new() { KhongTimThay = true, ThongBao = "Dữ liệu không tồn tại." };
}
