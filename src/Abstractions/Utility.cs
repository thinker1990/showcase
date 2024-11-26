using System.Net;

namespace Abstractions;

/// <summary>
/// Provides utility methods and properties.
/// </summary>
public static class Utility
{
    /// <summary>
    /// Gets the IP address for localhost.
    /// </summary>
    public static IPAddress Localhost { get; } = IPAddress.Loopback;

    /// <summary>
    /// Represents an action that does nothing.
    /// </summary>
    public static Action DoNothing { get; } = () => { };

    /// <summary>
    /// Generates a unique identifier.
    /// </summary>
    /// <returns>A unique identifier as a string.</returns>
    public static string UniqueId() => Guid.NewGuid().ToString("N");

    /// <summary>
    /// Ensures that the specified string is not null or empty.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <exception cref="ArgumentNullException">Thrown when the string value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the string value is empty or whitespace.</exception>
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
