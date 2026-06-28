using System.Diagnostics;
using System.Security;
using System.Text;
using MySugarBuddy.Application;

namespace MySugarBuddy.Desktop;

public class WindowsNotificationPort : INotificationPort
{
    private readonly INotificationPort _fallback;

    public WindowsNotificationPort(INotificationPort fallback)
    {
        _fallback = fallback;
    }

    public void Send(NotificationMessage message)
    {
        if (!OperatingSystem.IsWindows())
        {
            _fallback.Send(message);
            return;
        }

        try
        {
            using var process = Process.Start(CreateToastProcess(message));

            if (process is null || !process.WaitForExit(2000) || process.ExitCode != 0)
            {
                _fallback.Send(message);
            }
        }
        catch
        {
            _fallback.Send(message);
        }
    }

    private static ProcessStartInfo CreateToastProcess(NotificationMessage message)
    {
        var title = EscapePowerShellText(SecurityElement.Escape(message.Title) ?? message.Title);
        var body = EscapePowerShellText(SecurityElement.Escape(message.Body) ?? message.Body);
        var script = $@"
$title = '{title}'
$body = '{body}'

try {{
    [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null
    [Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] > $null

    $xml = New-Object Windows.Data.Xml.Dom.XmlDocument
    $xml.LoadXml(""<toast><visual><binding template='ToastGeneric'><text>$title</text><text>$body</text></binding></visual></toast>"")
    $toast = New-Object Windows.UI.Notifications.ToastNotification $xml
    $notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier('My Sugar Buddy')
    $notifier.Show($toast)
    exit 0
}} catch {{
    exit 1
}}
";

        return new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {EncodePowerShell(script)}",
            CreateNoWindow = true,
            UseShellExecute = false
        };
    }

    private static string EncodePowerShell(string script)
    {
        return Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
    }

    private static string EscapePowerShellText(string value)
    {
        return value.Replace("'", "''");
    }
}
