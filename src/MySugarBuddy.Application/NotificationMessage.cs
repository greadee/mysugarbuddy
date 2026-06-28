namespace MySugarBuddy.Application;

public class NotificationMessage
{
    public NotificationMessage(string title, string body)
    {
        Title = title;
        Body = body;
    }

    public string Title { get; }

    public string Body { get; }
}
