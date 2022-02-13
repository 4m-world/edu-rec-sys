namespace CodeMatrix.Mepd.Shared.Notifications;

public class JobNotification : INotificationMessage
{
    public string MessageType { get; set; } = typeof(JobNotification).Name;
    public string Message { get; set; } = default;
    public string JobId { get; set; } = default;
    public decimal Progress { get; set; }
}