namespace CodeMatrix.Mepd.Shared.Notifications;

public class BasicNotification : INotificationMessage
{
    public enum LabelType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public string MessageType { get; set; } = typeof(BasicNotification).Name;
    public string Message { get; set; } = default;
    public LabelType Label { get; set; }
}