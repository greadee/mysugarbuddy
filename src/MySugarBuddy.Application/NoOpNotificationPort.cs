namespace MySugarBuddy.Application;

public class NoOpNotificationPort : INotificationPort
{
    public static NoOpNotificationPort Instance { get; } = new();

    private NoOpNotificationPort()
    {
    }

    public void Send(NotificationMessage message)
    {
    }
}
