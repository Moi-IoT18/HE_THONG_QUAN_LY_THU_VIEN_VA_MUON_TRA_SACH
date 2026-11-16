using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;

namespace QuanLyThuVien.Tests;

public sealed class EmailNotificationTests
{
    [Fact]
    public async Task SendAsync_ReturnsDisabledWhenSmtpIsNotConfigured()
    {
        var service = new SmtpEmailNotificationService(
            Options.Create(new EmailOptions()),
            NullLogger<SmtpEmailNotificationService>.Instance);

        var result = await service.SendAsync("reader@example.test", "Test", "Test");

        Assert.Equal(EmailDeliveryStatus.Disabled, result.Status);
        Assert.Contains("chưa được gửi", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendAsync_RejectsInvalidAddressesWithoutContactingSmtp()
    {
        var service = new SmtpEmailNotificationService(
            Options.Create(new EmailOptions
            {
                Host = "smtp.example.test",
                From = "library@example.test",
            }),
            NullLogger<SmtpEmailNotificationService>.Instance);

        var result = await service.SendAsync("not-an-email", "Test", "Test");

        Assert.Equal(EmailDeliveryStatus.Failed, result.Status);
    }
}
