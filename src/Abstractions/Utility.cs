using System.Net;

namespace Abstractions;

public static class Utility
{
    public static IPAddress Localhost => IPAddress.Loopback;

    public static Action DoNothing => () => { };

    public static string UniqueId() => Guid.NewGuid().ToString("N");

    public static void EnsureNotEmpty(string value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} should not be empty.");
        }
    }
}