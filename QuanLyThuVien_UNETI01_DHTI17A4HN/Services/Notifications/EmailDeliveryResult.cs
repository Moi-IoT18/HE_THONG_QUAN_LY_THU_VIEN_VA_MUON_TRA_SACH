namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Notifications;

public enum EmailDeliveryStatus
{
    Sent,
    Disabled,
    Failed,
}

public sealed record EmailDeliveryResult(EmailDeliveryStatus Status, string Message);
