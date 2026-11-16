using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;

public sealed class SmtpEmailNotificationService : IEmailNotificationService
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailNotificationService> _logger;

    public SmtpEmailNotificationService(
        IOptions<EmailOptions> options,
        ILogger<SmtpEmailNotificationService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailDeliveryResult> SendAsync(string recipient, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.From))
            return new(EmailDeliveryStatus.Disabled, "Email chưa được gửi vì chưa cấu hình máy chủ SMTP.");
        if (_options.Port is < 1 or > 65535 || _options.TimeoutMilliseconds is < 1000 or > 120000
            || (string.IsNullOrWhiteSpace(_options.Username) != string.IsNullOrWhiteSpace(_options.Password)))
            return new(EmailDeliveryStatus.Disabled, "Email chưa được gửi vì cấu hình SMTP không hợp lệ.");

        if (!MailAddress.TryCreate(_options.From, out var sender)
            || !MailAddress.TryCreate(recipient, out var destination))
            return new(EmailDeliveryStatus.Failed, "Email chưa được gửi vì địa chỉ người nhận hoặc người gửi không hợp lệ.");
        using var message = new MailMessage(sender, destination)
        {
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = false,
        };
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Timeout = _options.TimeoutMilliseconds,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
        };
        if (!string.IsNullOrWhiteSpace(_options.Username))
            client.Credentials = new System.Net.NetworkCredential(_options.Username, _options.Password);

        try
        {
            await client.SendMailAsync(message);
            return new(EmailDeliveryStatus.Sent, "Đã gửi email thông báo.");
        }
        catch (SmtpException exception)
        {
            _logger.LogError(exception, "Unable to deliver a library workflow email.");
            return new(EmailDeliveryStatus.Failed, "Giao dịch đã lưu nhưng gửi email thất bại; hãy kiểm tra cấu hình SMTP.");
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "SMTP client could not send a library workflow email.");
            return new(EmailDeliveryStatus.Failed, "Giao dịch đã lưu nhưng gửi email thất bại; hãy kiểm tra cấu hình SMTP.");
        }
    }
}
