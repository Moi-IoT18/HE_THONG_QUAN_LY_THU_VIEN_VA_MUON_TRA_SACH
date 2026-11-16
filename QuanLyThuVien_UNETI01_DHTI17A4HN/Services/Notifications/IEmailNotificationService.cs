namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;

public interface IEmailNotificationService
{
    Task<EmailDeliveryResult> SendAsync(string recipient, string subject, string body);
}
