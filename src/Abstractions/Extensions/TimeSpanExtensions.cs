namespace Abstractions.Extensions;

/// <summary>
/// Provides extension methods for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Converts the specified number of seconds to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="seconds">The number of seconds.</param>
    /// <returns>A <see cref="TimeSpan"/> that represents the specified number of seconds.</returns>
    public static TimeSpan Seconds(this int seconds) =>
        TimeSpan.FromSeconds(seconds);

    /// <summary>
    /// Converts the specified number of milliseconds to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds.</param>
    /// <returns>A <see cref="TimeSpan"/> that represents the specified number of milliseconds.</returns>
    public static TimeSpan Milliseconds(this int milliseconds) =>
        TimeSpan.FromMilliseconds(milliseconds);
}
