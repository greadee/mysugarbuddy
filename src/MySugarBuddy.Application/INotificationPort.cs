namespace MySugarBuddy.Application;

public interface INotificationPort
{
    void Send(NotificationMessage message);
}
