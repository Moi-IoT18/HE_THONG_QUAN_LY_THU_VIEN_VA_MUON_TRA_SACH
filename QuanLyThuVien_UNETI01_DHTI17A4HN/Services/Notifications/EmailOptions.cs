namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;

public sealed class EmailOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? From { get; set; }
    public bool EnableSsl { get; set; } = true;
    public int TimeoutMilliseconds { get; set; } = 10000;
}
