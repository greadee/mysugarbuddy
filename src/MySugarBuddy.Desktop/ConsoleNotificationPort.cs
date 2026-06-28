using MySugarBuddy.Application;

namespace MySugarBuddy.Desktop;

public class ConsoleNotificationPort : INotificationPort
{
    public void Send(NotificationMessage message)
    {
        Console.WriteLine();
        Console.WriteLine($"Notification: {message.Title}");
        Console.WriteLine(message.Body);
    }
}
